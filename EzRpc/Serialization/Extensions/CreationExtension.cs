﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EzRpc.Serialization.Extensions
{
	public static class CreationExtension
	{
		private static readonly Dictionary<Type, Func<object>> CreationCache = new Dictionary<Type, Func<object>>();

		/// <summary>
		/// Quick creation of new instance
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object? NewInstance(this Type type)
		{
			if (type.IsPrimitive)
			{
				return Activator.CreateInstance(type);
			}
			if (type.IsAbstract)
			{
				return null;
			}
			Func<object> create = GetOrCreate(type);
			return create();
		}
		
		public static Func<object> GetOrCreate(Type type)
		{
			Func<object> create;
			if (CreationCache.TryGetValue(type, out create) == false)
			{
				create = GetActivator(type);
				CreationCache[type] = create;
			}
			return create;
		}

		public static Func<T> GetActivator<T>()
			=> Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
		
		public static Func<object> GetActivator(Type type)
			=> Expression.Lambda<Func<object>>(Expression.New(type)).Compile();
	}
}
