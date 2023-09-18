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
		
		public MethodInfo? GetMethod(Type type, string name)
		{
			if (_methodContainer[type]?.TryGet(name, out MethodInfo method) == true)
			{
				return method;
			}
			Log.Warn("Failed to find method {0} on type {1}", name, type?.Name);
			return null;
		}
		
		public Synced? GetMethodSyncData(Type type, string name)
		{
			if (_methodContainer[type]?.TryGet(name, out Synced method) == true)
			{
				return method;
			}
			Log.Warn("Failed to find method {0} on type {1}", name, type?.Name);
			return null;
		}
	}
}
