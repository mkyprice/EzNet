using EzNet.IO.Extensions;
using EzNet.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace EzRpc.Serialization.Extensions
{
	public static class SerializationExtension
	{
		private static readonly ConcurrentDictionary<string, Type> TypeCache = new ConcurrentDictionary<string, Type>();
		
		public static void Serialize(this Stream stream, object value)
		{
			Type type = value.GetType();
			string fullname = type.FullName;
			TypeCache[fullname] = type;
			stream.Write(fullname);
			Rpc.GetSerializer(type).Serialize(stream, value);
		}

		public static object? Deserialize(this Stream stream)
		{
			string fullname = stream.ReadString();
			if (fullname != null)
			{
				Type type;
				if (TypeCache.TryGetValue(fullname, out type) == false)
				{
					try
					{
						type = Type.GetType(fullname, true);
					}
					catch (Exception e)
					{
						Log.Warn("Failed to find type {0}\n{1}\n{2}", fullname, e.Message, e.StackTrace);
					}
					if (type != null)
					{
						TypeCache[fullname] = type;
					}
				}
				if (type != null)
				{
					return Rpc.GetSerializer(type).Deserialize(stream, type);
				}
			}
			return null;
		}
		
		public static void WritePrimitive(this Stream stream, object value)
		{
			switch (value)
			{
				case bool v:
					stream.Write(v);
					break;
				case string v:
					stream.Write(v);
					break;
				case byte v:
					stream.Write(v);
					break;
				case sbyte v:
					stream.Write(v);
					break;
				case short v:
					stream.Write(v);
					break;
				case ushort v:
					stream.Write(v);
					break;
				case int v:
					stream.Write(v);
					break;
				case uint v:
					stream.Write(v);
					break;
				case long v:
					stream.Write(v);
					break;
				case ulong v:
					stream.Write(v);
					break;
				case float v:
					stream.Write(v);
					break;
				case double v:
					stream.Write(v);
					break;
				case char v:
					stream.Write(v);
					break;
				case decimal v:
					stream.Write(v);
					break;
				case byte[] v:
					stream.Write(v, 0, v.Length);
					break;
				case DateTime v:
					stream.Write(v.ToBinary());
					break;
				default:
					Log.Warn("Unregonized type {0} could not be serialized", value);
					break;
			}
		}

		public static object ReadPrimitive(this Stream stream, Type type)
		{
			TypeCode tc = Type.GetTypeCode(type);
			switch (tc)
			{
				case TypeCode.Boolean:
					return stream.ReadBool();
				case TypeCode.String:
					return stream.ReadString();
				case TypeCode.Byte:
					return (byte)stream.ReadByte();
				case TypeCode.SByte:
					return stream.ReadSByte();
				case TypeCode.Int16:
					return stream.ReadShort();
				case TypeCode.UInt16:
					return stream.ReadUShort();
				case TypeCode.Int32:
					return stream.ReadInt();
				case TypeCode.UInt32:
					return stream.ReadUInt();
				case TypeCode.Int64:
					return stream.ReadLong();
				case TypeCode.UInt64:
					return stream.ReadULong();
				case TypeCode.Single:
					return stream.ReadSingle();
				case TypeCode.Double:
					return stream.ReadDouble();
				case TypeCode.Decimal:
					return stream.ReadDecimal();
				case TypeCode.Char:
					return stream.ReadChar();
				case TypeCode.DateTime:
					return DateTime.FromBinary(stream.ReadLong());
				default:
					Log.Warn("Could not deserialize type {0}", type);
					break;
			}
			return default;
		}
	}
}
