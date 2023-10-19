using EzNet;
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

		public RpcClient() : base(new RpcSession()) { }

		public RpcClient(Connection tcp, Connection udp) : this(tcp, udp, new RpcSession())
		{
		}
		
		public RpcClient(Connection tcp, Connection udp, RpcSession session) : base(tcp, udp, session)
		{
			Tcp = tcp;
			Udp = udp;
		}

		public async Task<TR> CallAsync<T, TR>(string method, params object[] args)
			=> await CallAsync<TR>(typeof(T), method, args);

		public async Task<T> CallAsync<T>(Type type, string method, params object[] args)
		{
			if (HandleLocalCall(type, method, args, out RpcRequest request, out Network sender) &&
			    sender is Connection connection)
			{
				
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
			return default;
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
