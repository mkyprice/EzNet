using EzRPC.Logging;
using EzRPC.Reflection.Core;
using System;
using System.Reflection;

namespace EzRPC.Reflection.Extensions
{
	internal static class MethodExtensions
	{
		/// <summary>
		/// Call a method on an object
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static object CallMethod(object obj, string method, params object[] args)
		{
			MethodInfo? info = GetMethod(obj.GetType(), method);
			if (info != null)
			{
				return info.Invoke(obj, args);
			}
			return null;
		}
		
		/// <summary>
		/// Invoke a static method
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static object CallMethod(Type type, string method, params object[] args)
		{
			MethodInfo? info = GetMethod(type, method);
			if (info != null)
			{
				return info.Invoke(null, args);
			}
			return null;
		}
		
		public static MethodInfo? GetMethod(Type type, string name)
		{
			if (ReflectionCache.MethodCache[type]?.TryGet(name, out MethodInfo method) == true)
			{
				return method;
			}
			Log.Warn("Failed to find method {0} on type {1}", name, type?.Name);
			return null;
		}
	}
}
