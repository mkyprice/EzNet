using EzNet.Messaging;

namespace EzNet.IO
{
	public static class StreamByteWriterExtension
	{
		public static void Write(this Stream stream, char value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
		}
		
		public static void Write(this Stream stream, byte value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte(value);
		}
		
		public static void Write(this Stream stream, sbyte value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
		}
		
		public static void Write(this Stream stream, short value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
		}
		
		public static void Write(this Stream stream, ushort value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
		}
		
		public static void Write(this Stream stream, int value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}
		
		public static void Write(this Stream stream, uint value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
		}
		
		public static void Write(this Stream stream, long value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
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
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)value);
			stream.WriteByte((byte)(value >> 8));
			stream.WriteByte((byte)(value >> 16));
			stream.WriteByte((byte)(value >> 24));
			stream.WriteByte((byte)(value >> 32));
			stream.WriteByte((byte)(value >> 40));
			stream.WriteByte((byte)(value >> 48));
			stream.WriteByte((byte)(value >> 56));
		}
		
		public static void Write(this Stream stream, float value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			int v = (int)value;
			stream.WriteByte((byte)v);
			stream.WriteByte((byte)(v >> 8));
			stream.WriteByte((byte)(v >> 16));
			stream.WriteByte((byte)(v >> 24));
		}
		
		public static void Write(this Stream stream, double value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			long v = (long)value;
			stream.WriteByte((byte)v);
			stream.WriteByte((byte)(v >> 8));
			stream.WriteByte((byte)(v >> 16));
			stream.WriteByte((byte)(v >> 24));
			stream.WriteByte((byte)(v >> 32));
			stream.WriteByte((byte)(v >> 40));
			stream.WriteByte((byte)(v >> 48));
			stream.WriteByte((byte)(v >> 56));
		}
		
		public static void Write(this Stream stream, bool value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			stream.WriteByte((byte)(value ? 0 : 1));
		}
		
		private static void Write7BitEncodedInt(this Stream stream, int value)
		{
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
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
			if (stream.CanWrite == false) throw new ArgumentException("Cannot write to stream");
			int length = value.Length;
			stream.Write7BitEncodedInt(length);
			byte[] bytes = BasePacket.Encoding.GetBytes(value);
			stream.Write(bytes);
		}
	}
}
