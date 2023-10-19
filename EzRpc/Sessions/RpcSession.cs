using EzRpc.Logging;
using EzRPC.Reflection.Core;
using EzRpc.Sessions.Instances;
using System;
using System.Reflection;

namespace EzRpc.State
{
	public class RpcSession
	{
		private readonly MethodContainer _methodContainer = new MethodContainer();
		private readonly InstanceCache _instances = new InstanceCache();
		
		public void Bind<T>(T obj)
		{
			Bind<T>();
			_instances.Add(obj);
		}

		public void Bind<T>()
		{
			_methodContainer.Cache(typeof(T));
		}

		public bool TryGetInstance(Type type, out object value) => _instances.TryGet(type, out value);
		
		public bool TryGetMethod(Type type, string name, out MethodInfo? info)
		{
			if (_methodContainer.TryGetMethodCache(type, out MethodCache? cache) &&
				cache.TryGet(name, out info))
			{
				return true;
			}
			Log.Warn("Failed to find method {0} on type {1}", name, type?.Name);
			info = null;
			return false;
		}
		
		public bool TryGetMethodSyncData(Type type, string name, out Synced? synced)
		{
			if (_methodContainer.TryGetMethodCache(type, out MethodCache cache) &&
			    cache.TryGet(name, out synced))
			{
				return true;
			}
			Log.Warn("Failed to find method {0} on type {1}", name, type?.Name);
			synced = null;
			return false;
		}
	}
}
