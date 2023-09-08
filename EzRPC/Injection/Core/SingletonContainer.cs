using EzRPC.Injection.Abstractions;
using EzRPC.Logging;
using System;
using System.Collections.Generic;

namespace EzRPC.Injection.Core
{
	public class SingletonContainer : IServiceContainer
	{
		private readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
		
		public bool Add<T>(T value)
			where T : notnull
		{
			Type type = typeof(T);
			if (_objects.ContainsKey(type))
			{
				Log.Warn("Type {0} was already added", type);
				return false;
			}

			if (type.IsInterface)
			{
				_objects[type] = value;
			}
			else
			{
				Type[] interfaces = type.GetInterfaces();
				foreach (Type i in interfaces)
				{
					_objects[i] = value;
				}
			}
			return true;
		}
		
		public T Get<T>()
		{
			if (_objects.TryGetValue(typeof(T), out object value))
			{
				return (T)value;
			}
			return default(T);
		}
		
		public object? Get(Type type)
		{
			if (_objects.TryGetValue(type, out object value))
			{
				return value;
			}
			return null;
		}
		public bool Contains<T>() => _objects.ContainsKey(typeof(T));
	}
}
