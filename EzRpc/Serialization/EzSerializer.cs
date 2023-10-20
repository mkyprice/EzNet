using EzNet.Logging;
using EzNet.Serialization.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EzRpc.Serialization
{
	public class EzSerializer : ISerializer
	{
		public static BindingFlags Flags = BindingFlags.Default | 
		                                   BindingFlags.Instance | 
		                                   BindingFlags.Public | 
		                                   BindingFlags.NonPublic;
		private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new Dictionary<Type, FieldInfo[]>();
		
		public void Serialize(Stream stream, object value)
		{
			Type type = value?.GetType();
			if (type == null) return;
			foreach (FieldInfo field in GetFields(type))
			{
				object v = field.GetValue(value);
				if (IsPrimitive(field.FieldType))
				{
					stream.WritePrimitive(v);
				}
				else
				{
					Serialize(stream, v);
				}
			}
		}

		public object Deserialize(Stream stream, Type type)
		{
			object? value = type?.NewInstance();
			if (value == null)
			{
				Log.Warn("Failed to create instance of {0}", type);
				return default;
			}
			foreach (FieldInfo field in GetFields(type))
			{
				object v;
				if (IsPrimitive(field.FieldType))
				{
					v = stream.ReadPrimitive(field.FieldType);
				}
				else
				{
					v = Deserialize(stream, field.FieldType);
				}
				field.SetValue(value, v);
			}
			return value;
		}

		private static FieldInfo[] GetFields(Type type)
		{
			if (FieldCache.ContainsKey(type) == false)
			{
				FieldCache[type] = type.GetFields(Flags);
			}
			return FieldCache[type];
		}

		private static bool IsPrimitive(Type type)
		{
			TypeCode tc = Type.GetTypeCode(type);
			return tc != TypeCode.Empty && tc != TypeCode.Object && tc != TypeCode.DBNull;
		}
	}
}
