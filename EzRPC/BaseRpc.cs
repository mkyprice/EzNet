using EzRPC.Reflection;
using EzRPC.Reflection.Extensions;
using EzRPC.Transport;
using System;
using System.Threading.Tasks;

namespace EzRPC
{
	public abstract class BaseRpc : IDisposable
	{
		private readonly NetworkPacketQueue _incomingPacketQueue = new NetworkPacketQueue();
		protected bool IsDisposed { get; private set; } = false;

		public BaseRpc()
		{
			if (Rpc.IsInitialized() == false)
			{
				Rpc.Initialize();
			}
		}
		
		public async Task<object> CallAsync<T>(string method, params object[] args) => await CallAsync(typeof(T), method, args);

		public async Task<object> CallAsync(Type type, string method, params object[] args)
		{
			RpcRequest request = new RpcRequest()
			{
				RequestId = Guid.NewGuid().ToString().GetHashCode(),
				Method = new MethodModel()
				{
					DeclaringType = type,
					MethodName = method,
					ParameterTypes = args.ToTypeArray()
				},
				Params = args
			};
			
			return MethodExtensions.CallMethod(type, method, args);
		}

		public async Task<object> CallAsync(object instance, string method, params object[] args)
		{
			return MethodExtensions.CallMethod(instance, method, args);
		}

		protected abstract Task<object> SendRequestAsync(RpcRequest request);
		
		public virtual void Dispose()
		{
			if (IsDisposed) return;
			IsDisposed = true;
		}
	}
}
