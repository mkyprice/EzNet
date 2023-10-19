using EzNet;
using EzRpc.Logging;
using EzRpc.Messaging;
using EzRpc.State;
using System;
using System.Reflection;

namespace EzRpc
{
	public abstract class Rpc : IDisposable
	{
		protected readonly RpcSession Session;
		protected readonly Network Tcp;
		protected readonly Network Udp;

		public Rpc(Network tcp, Network udp, RpcSession session)
		{
			Tcp = tcp;
			Udp = udp;
			Session = session;
			
			Network.RegisterPacket<RpcRequest>();
			Network.RegisterPacket<RpcResponse>();
			Tcp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
		}

		/// <summary>
		/// Bind object as handler
		/// </summary>
		/// <param name="obj"></param>
		/// <typeparam name="T"></typeparam>
		public void Bind<T>(T obj) => Session.Bind(obj);
		
		/// <summary>
		/// Bind type as handler
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void Bind<T>() => Session.Bind<T>();

		/// <summary>
		/// Call method without awaiting response
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <typeparam name="T"></typeparam>
		public void Call<T>(string method, params object[] args) => Call(typeof(T), method, args);
		
		/// <summary>
		/// Call method without awaiting response
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <typeparam name="T"></typeparam>
		public abstract void Call(Type type, string method, params object[] args);
		
		/// <summary>
		/// Call a local method
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		protected object? CallLocalMethod(Type type, string method, params object[] args)
		{
			if (Session.TryGetMethod(type, method, out MethodInfo info))
			{
				Session.TryGetInstance(type, out object? instance);
				return info.Invoke(instance, args);
			}
			return null;
		}

		/// <summary>
		/// Performs all Call operations
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <param name="request">Filled out RPC request</param>
		/// <param name="send">The connection to send through</param>
		/// <returns></returns>
		protected bool HandleLocalCall(Type type, string method, object[] args, out RpcRequest request, out Network send)
		{
			if (Session.TryGetMethodSyncData(type, method, out Synced? sync) == false)
			{
				request = default;
				send = null;
				Log.Warn("No bound instance for type {0}", type);
				return false;
			}
			// Local
			if (sync.CallLocal)
			{
				CallLocalMethod(type, method, args);
			}
			
			// Send request
			send = sync.IsReliable ? Tcp : Udp;
			request = new RpcRequest()
			{
				Type = type,
				Method = method,
				Args = args
			};
			return true;
		}

		/// <summary>
		/// Handles all incoming RpcRequests
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private RpcResponse ResponseHandler(RpcRequest request)
		{
			object? result = CallLocalMethod(request.Type, request.Method, request.Args);
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
