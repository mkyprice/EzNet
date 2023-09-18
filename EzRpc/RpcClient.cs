using EzNet;
using EzRpc.Logging;
using EzRpc.Messaging;
using EzRpc.State;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EzRpc
{
	public class RpcClient : Rpc
	{
		protected new Connection Tcp;
		protected new Connection Udp;
		
		public RpcClient(Connection tcp, Connection udp) : this(tcp, udp, new RpcSession())
		{
		}
		
		public RpcClient(Connection tcp, Connection udp, RpcSession session) : base(tcp, udp, session)
		{
			Tcp = tcp;
			Udp = udp;
		}

		public async Task<T> CallAsync<T>(Type type, string method, params object[] args)
		{
			MethodInfo? info = Session.GetMethod(type, method);
			Synced? sync = Session.GetMethodSyncData(type, method);
			if (info == null || sync == null)
			{
				Log.Warn("No bound instance for type {0}", type);
				return default;
			}
			// Local
			if (sync.CallLocal)
			{
				CallMethod(type, method, args);
			}

			// Send request
			Connection sender = sync.IsReliable ? Tcp : Udp;
			RpcRequest request = new RpcRequest()
			{
				Type = type,
				Method = method,
				Args = args
			};
			RpcResponse response = await sender.SendAsync<RpcResponse, RpcRequest>(request);
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
	}
}
