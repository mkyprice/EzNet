using EzRpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EzRPC.Reflection.Core
{
	internal sealed class MethodCache
	{
		public readonly Type CacheType;

		private static readonly BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | 
		                                             BindingFlags.NonPublic;
		private readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>();
		private readonly Dictionary<string, Synced> _methodData = new Dictionary<string, Synced>();
		
		public MethodCache(Type type)
		{
			CacheType = type;
			ReadMethods();
		}

		public MethodInfo this[string name] { get => _methods[name]; }

		public bool TryGet(string name, out MethodInfo method) => _methods.TryGetValue(name, out method);
		public bool TryGet(string name, out Synced method) => _methodData.TryGetValue(name, out method);

		private void ReadMethods()
		{
			var methods = CacheType.GetMethods(Flags);
			foreach (MethodInfo method in methods)
			{
				_methods[method.Name] = method;

				Synced sync = method.GetCustomAttribute<Synced>() ?? new Synced();
				_methodData[method.Name] = sync;
			}
		}
	}
}
