using EzNet.Messaging;
using System.IO;

namespace EzNet.IO
{
	public static class StreamByteWriterExtension
	{
		public static void Write(this Stream stream, char value)
		{
			stream.WriteByte((byte)value);
		}
		
		public static void Write(this Stream stream, byte value)
		{
			stream.WriteByte(value);
		}
		
		public static void Write(this Stream stream, sbyte value)
		{
			stream.WriteByte((byte)value);
		}
		
		public static void Write(this Stream stream, short value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
		}
		
		public static void Write(this Stream stream, ushort value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
		}
		
		public static void Write(this Stream stream, int value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}
		
		public static void Write(this Stream stream, uint value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}
		
		public static void Write(this Stream stream, long value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
			stream.WriteByte((byte)(value >> 32));
			stream.WriteByte((byte)(value >> 40));
			stream.WriteByte((byte)(value >> 48));
			stream.WriteByte((byte)(value >> 56));
		}
		
		public static void Write(this Stream stream, ulong value)
		{
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
			stream.WriteByte((byte)(value >> 32));
			stream.WriteByte((byte)(value >> 40));
			stream.WriteByte((byte)(value >> 48));
			stream.WriteByte((byte)(value >> 56));
		}
		
		public static unsafe void Write(this Stream stream, float value)
		{
			uint v = *(uint*)&value;
			stream.Write(v);
		}
		
		public static unsafe void Write(this Stream stream, double value)
		{
			ulong v = *(ulong*)&value;
			stream.Write(v);
		}
		
		public static void Write(this Stream stream, bool value)
		{
			stream.WriteByte((byte)(value ? 0 : 1));
		}
		
		private static void Write7BitEncodedInt(this Stream stream, int value)
		{
			uint count = (uint)value;
			while (count >= 0x80)
			{
				stream.WriteByte((byte)(count | 0x80));
				count >>= 7;
			}
			stream.WriteByte((byte)count);
		}
		
		public static void Write(this Stream stream, string value)
		{
			int length = value.Length;
			stream.Write7BitEncodedInt(length);
			byte[] bytes = BasePacket.Encoding.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
		}
	}
}
