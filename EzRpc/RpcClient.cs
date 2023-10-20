using EzNet;
using EzRpc.Injection;
using EzRpc.Logging;
using EzRpc.Messaging;
using EzRpc.State;
using System;
using System.Threading.Tasks;

namespace EzRpc
{
	public class RpcClient : Rpc
	{
		public new Connection? Tcp { get => base.Tcp as Connection; set => base.Tcp = value; }
		public new Connection? Udp { get => base.Udp as Connection; set => base.Udp = value; }

		public RpcClient() : base(new RpcSession()) 
		{ }

		public RpcClient(Connection tcp, Connection udp) : this(tcp, udp, new RpcSession())
		{ }
		public RpcClient(Connection tcp, Connection udp, RpcSession session) : base(tcp, udp, session)
		{ }

		/// <summary>
		/// Call remote function with response
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <returns></returns>
		public async Task<TR> CallAsync<T, TR>(string method, params object[] args)
			=> await CallAsync<TR>(typeof(T), method, args);

		/// <summary>
		/// Call remote function with response
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public async Task<T> CallAsync<T>(Type type, string method, params object[] args)
		{
			if (HandleLocalCall(type, method, args, out RpcRequest request, out Network sender) == false ||
			    (sender is Connection connection) == false) return default;
			
			RpcResponse response = await connection.SendAsync<RpcResponse, RpcRequest>(request);
			if (response.Error != RPC_ERROR.None)
			{
				Log.Warn("RPC encountered error: {0}", response.Error);
				return default;
			}
			if (response.Result == null)
			{
				Log.Warn("Result was null");
				return default;
			}
			return (T)response.Result;
		}
		
		public override void Call(Type type, string method, params object[] args)
		{
			if (HandleLocalCall(type, method, args, out RpcRequest request, out Network sender) &&
			    sender is Connection connection)
			{
				connection.Send(request);
			}
		}
	}
}
