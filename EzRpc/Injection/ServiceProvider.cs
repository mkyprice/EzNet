using EzNet.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EzRpc.Injection
{
	/// <summary>
	/// Basic DI
	/// </summary>
	public class ServiceProvider : IDisposable
	{
		/// <summary>
		/// Maps interface types to the bound type
		/// </summary>
		private readonly ConcurrentDictionary<Type, Type> _trueTypeMap = new ConcurrentDictionary<Type, Type>();
		/// <summary>
		/// Contains all singletons
		/// </summary>
		private readonly ConcurrentDictionary<Type, object?> _singletonCache = new ConcurrentDictionary<Type, object?>();
		
		public void AddTransient<T>()
			where T : class
		{
			Type type = typeof(T);
			_trueTypeMap[type] = type;
		}

		public void AddTransient<IT, T>()
			where T : class
		{
			Type interfaceType = typeof(IT);
			Type implementation = typeof(T);
			if (interfaceType.IsAssignableFrom(implementation) == false)
			{
				throw new ArgumentException($"Type {implementation} must be assignable to type {interfaceType}");
			}
			_trueTypeMap[interfaceType] = implementation;
			AddTransient<T>();
		}

		public void AddSingleton<T>()
			where T : class
		{
			Type type = typeof(T);
			AddTransient<T>();
			_trueTypeMap[type] = type;
			_singletonCache[type] = null;
		}

		public void AddSingleton<IT, T>()
			where T : class
		{
			Type interfaceType = typeof(IT);
			Type implementation = typeof(T);
			if (interfaceType.IsAssignableFrom(implementation) == false)
			{
				throw new ArgumentException($"Type {implementation} must be assignable to type {interfaceType}");
			}
			_singletonCache[interfaceType] = null;
			_trueTypeMap[interfaceType] = implementation;
			AddSingleton<T>();
		}

		public T? GetService<T>()
			where T : class
			=> GetService(typeof(T)) as T;

		public object? GetService(Type type)
		{
			type = _trueTypeMap[type];
			return GetInstance(type);
		}
		
		public void Dispose()
		{
			_singletonCache.Clear();
			_trueTypeMap.Clear();
		}

		/// <summary>
		/// Quick creation of new instance
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private object? GetInstance(Type type)
		{
			object? value;
			if (_singletonCache.TryGetValue(type, out value) && value != null)
			{
				return value;
			}
			ConstructorInfo constructorInfo = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).First();
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			if (parameters.Length > 0)
			{
				object[] paramObjs = new object[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					object? p = GetInstance(parameters[i].ParameterType);
					if (p == null)
					{
						Log.Error("Could not create instance of parameter {0}", parameters[i].ParameterType);
					}
					paramObjs[i] = p;
				}
				value = constructorInfo.Invoke(paramObjs);
			}
			else
			{
				value = constructorInfo.Invoke(Array.Empty<object>());
			}
			if (_singletonCache.ContainsKey(type))
			{
				_singletonCache[type] = value;
			}
			return value;
		}
	}
}
