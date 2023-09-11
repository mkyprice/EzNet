using EzNet.IO.Extensions;
using EzRPC.Logging;
using System;
using System.IO;

namespace EzRPC.Serialization.Extensions
{
	public static class SerializationExtension
	{
		public static void WriteObject(this Stream stream, object value)
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
				default:
					Log.Warn("Unregonized type {0} could not be serialized", value);
					break;
			}
		}

		public static object Read(this Stream stream, Type type)
		{
			switch (type.Name)
			{
				case nameof(Boolean):
					return stream.ReadBool();
				case nameof(String):
					return stream.ReadString();
				case nameof(Byte):
					return stream.ReadByte();
				case nameof(SByte):
					return stream.ReadSByte();
				case nameof(Int16):
					return stream.ReadShort();
				case nameof(UInt16):
					return stream.ReadUShort();
				case nameof(Int32):
					return stream.ReadInt();
				case nameof(UInt32):
					return stream.ReadUInt();
				case nameof(Int64):
					return stream.ReadLong();
				case nameof(UInt64):
					return stream.ReadULong();
				case nameof(Single):
					return stream.ReadSingle();
				case nameof(Double):
					return stream.ReadDouble();
				case nameof(Decimal):
					return stream.ReadDecimal();
				case nameof(Char):
					return stream.ReadChar();
				default:
					Log.Warn("Could not deserialize type {0}", type);
					break;
			}
			return default;
		}
	}
}
