using EzRPC.Serialization.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EzRPC.Serialization
{
	/// <summary>
	/// Caches data types for faster serialization
	/// </summary>
	public class EzSerializer : IDisposable
	{
		private static readonly ConcurrentDictionary<Type, SerializationCache> Caches = 
			new ConcurrentDictionary<Type, SerializationCache>();

		public void Serialize(Stream stream, object value)
		{
			SerializationCache cache = GetCache(value);

			foreach (FieldInfo field in cache.Fields)
			{
				object v = field.GetValue(value);
				if (field.FieldType.IsPrimitive ||
				    field.FieldType == typeof(string))
				{
					stream.WriteObject(v);
				}
				else
				{
					Serialize(stream, v);
				}
			}
		}

		public object Deserialize(Stream stream, Type type)
		{
			object value = Activator.CreateInstance(type);
			SerializationCache cache = GetCache(value);

			foreach (FieldInfo field in cache.Fields)
			{
				object v;
				if (field.FieldType.IsPrimitive ||
				    field.FieldType == typeof(string))
				{
					v = stream.Read(field.FieldType);
				}
				else
				{
					v = Deserialize(stream, field.FieldType);
				}
				field.SetValue(value, v);
			}
			return value;
		}

		private SerializationCache GetCache(object value)
		{
			Type type = value.GetType();
			if (Caches.TryGetValue(type, out SerializationCache cache)) return cache;
			cache = new SerializationCache(type);
			Caches[type] = cache;
			return cache;
		}

		public void Dispose()
		{
			Caches.Clear();
		}
	}
}
