using EzRpc.Logging;
using System;
using System.Collections.Generic;

namespace EzRPC.Reflection.Core
{
	internal sealed class MethodContainer
	{
		private readonly Dictionary<Type, MethodCache> _caches = new Dictionary<Type, MethodCache>();

		public bool TryGetMethodCache(Type type, out MethodCache cache) => _caches.TryGetValue(type, out cache);

		public bool Contains(Type type) => _caches.ContainsKey(type);

		public bool Cache(Type type)
		{
			if (type == null)
			{
				Log.Warn("Tried to cache null type {0}");
				return false;
			}
			if (_caches.ContainsKey(type))
			{
				Log.Warn("Tried to cache existing type {0}", type.Name);
				return false;
			}

			_caches[type] = new MethodCache(type);
			return true;
		}
	}
}
