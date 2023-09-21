using System;
using System.Collections.Concurrent;

namespace EzRpc.Sessions.Instances
{
	public class InstanceCache
	{
		private readonly ConcurrentDictionary<Type, object> _instances = new ConcurrentDictionary<Type, object>();

		public bool Add<T>(T instance)
		{
			Type type = typeof(T);
			return _instances.TryAdd(type, instance);
		}

		public bool Remove<T>()
		{
			return _instances.TryRemove(typeof(T), out _);
		}

		public bool TryGet(Type type, out object value) => _instances.TryGetValue(type, out value);
	}
}
