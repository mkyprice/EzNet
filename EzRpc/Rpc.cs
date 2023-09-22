using EzNet;
using EzRpc.Injection;
using EzRpc.Logging;
using EzRpc.Messaging;
using EzRpc.State;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace EzRpc
{
	public abstract class Rpc : IDisposable
	{
		protected readonly ServiceProvider Services;
		protected readonly RpcSession Session;
		protected readonly Network Tcp;
		protected readonly Network Udp;

		public Rpc(Network tcp, Network udp, RpcSession session, ServiceProvider services)
		{
			Tcp = tcp;
			Udp = udp;
			Session = session;
			Services = services;
			
			Network.RegisterPacket<RpcRequest>();
			Network.RegisterPacket<RpcResponse>();
			Tcp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
		}

		public void Bind<T>()
			where T : class
		{
			Services.AddTransient<T>();
			Session.Bind<T>();
		}
		
		protected object? CallMethod(Type type, string method, params object[] args)
		{
			MethodInfo? info = Session.GetMethod(type, method);
			Synced? sync = Session.GetMethodSyncData(type, method);
			if (info == null || sync == null)
			{
				Log.Warn("No bound instance for type {0}", type);
				return null;
			}
			object? instance = Services.GetService(type);
			return info.Invoke(instance, args);
		}

		private RpcResponse ResponseHandler(RpcRequest request)
		{
			object? result = CallMethod(request.Type, request.Method, request.Args);
			return new RpcResponse()
			{
				Result = result
			};
		}
		
		public void Dispose()
		{
			Tcp?.Dispose();
			Udp?.Dispose();
		}
	}
}
