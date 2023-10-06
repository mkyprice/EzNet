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

		public void Bind<T>()
		{
			_methodContainer.Cache(typeof(T));
		}
		
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
