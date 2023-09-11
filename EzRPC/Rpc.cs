using EzNet;
using EzRPC.Logging;
using EzRPC.Reflection;
using EzRPC.Reflection.Core;
using EzRPC.Reflection.Extensions;
using EzRPC.Serialization;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EzRPC
{
	public abstract class Rpc : IDisposable
	{
		protected Network Tcp;
		protected Network Udp;
		protected readonly EzSerializer Serializer = new EzSerializer();
		private static int _nextRequestId = 1;
		
		protected bool IsDisposed { get; private set; } = false;

		public Rpc()
		{
			if (ReflectionCache.IsInitialized() == false)
			{
				ReflectionCache.Initialize();
			}
		}

		public abstract void Start(EndPoint tcp, EndPoint udp);
        
		protected RpcResponse ResponseHandler(RpcRequest request)
		{
			string method = request.MethodName;
			Type type = request.DeclaringType;
			object[] args = request.Params;

			object response = MethodExtensions.CallMethod(type, method, args);

			return new RpcResponse(response, RPC_ERROR.None);
		}
		
		public async Task<object> CallAsync<T>(string method, params object[] args) => await CallAsync(typeof(T), method, args);

		public async Task<object> CallAsync(Type type, string method, params object[] args)
		{
			RpcRequest request = new RpcRequest()
			{
				MethodName = method,
				DeclaringType = type,
				Params = args
			};

			RpcResponse response = await Tcp.SendAsync<RpcResponse, RpcRequest>(request);
			if (response.Error != RPC_ERROR.None)
			{
				Log.Warn("RPC call failed with error {0}", response.Error);
			}
			return response.Result;
		}

		// public async Task<object> CallAsync(object instance, string method, params object[] args)
		// {
		// 	return MethodExtensions.CallMethod(instance, method, args);
		// }
		
		public virtual void Dispose()
		{
			if (IsDisposed) return;
			IsDisposed = true;
			Serializer.Dispose();
		}
	}
}
