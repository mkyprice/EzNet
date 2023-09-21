using EzRpc.Logging;
using System;
using System.Collections.Generic;

namespace EzRPC.Reflection.Core
{
	internal sealed class MethodContainer
	{
		private readonly Dictionary<Type, MethodCache> _caches = new Dictionary<Type, MethodCache>();

		public MethodCache this[Type type]
		{
			get
			{
				if (_caches.TryGetValue(type, out MethodCache cache))
				{
					return cache;
				}
				Log.Warn("Tried to get unbound cache {0}", type);
				return null;
			}
		}

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
