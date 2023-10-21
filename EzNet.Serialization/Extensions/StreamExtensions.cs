namespace EzNet.Serialization.Extensions
{
	
	internal static class StreamExtensions
	{
		public static byte ReadByte(this Stream stream)
		{
			return (byte)stream.ReadByte();
		}
		
		public static sbyte ReadSByte(this Stream? stream)
		{
			return (sbyte)stream.ReadByte();
		}
		
		public static short ReadShort(this Stream? stream)
		{
			return (short)(stream.ReadByte() | 
			               stream.ReadByte() << 8);
		}
		
		public static ushort ReadUShort(this Stream? stream)
		{
			return (ushort)(stream.ReadByte() | 
			               stream.ReadByte() << 8);
		}
		
		public static int ReadInt(this Stream? stream)
		{
			return (int)(stream.ReadByte() | 
			               stream.ReadByte() << 8 |
			               stream.ReadByte() << 16 |
			               stream.ReadByte() << 24
			               );
		}
		
		public static uint ReadUInt(this Stream? stream)
		{
			return (uint)(stream.ReadByte() | 
			               stream.ReadByte() << 8 |
			               stream.ReadByte() << 16 |
			               stream.ReadByte() << 24
			               );
		}
		
		public static long ReadLong(this Stream? stream)
		{
			uint low = stream.ReadUInt();
			uint high = stream.ReadUInt();
			return (long)((ulong)high << 32 | low);
		}
		
		public static ulong ReadULong(this Stream? stream)
		{
			uint low = stream.ReadUInt();
			uint high = stream.ReadUInt();
			return (ulong)((ulong)high << 32 | low);
		}
		
		public static unsafe float ReadSingle(this Stream? stream)
		{
			uint iv = stream.ReadUInt();
			return *((float*)&iv);
		}
		
		public static decimal ReadDecimal(this Stream? stream)
		{
			int[] buffer = new int[4];
			buffer[0] = stream.ReadInt();
			buffer[1] = stream.ReadInt();
			buffer[2] = stream.ReadInt();
			buffer[3] = stream.ReadInt();
			return new decimal(buffer);
		}
		
		public static unsafe double ReadDouble(this Stream? stream)
		{
			ulong lv = stream.ReadULong();
			return *((double*)&lv);
		}
		
		public static bool ReadBool(this Stream? stream)
		{
			return stream.ReadByte() == 1;
		}
		
		public static char ReadChar(this Stream? stream)
		{
			return (char)stream.ReadByte();
		}
		
		public static byte[] ReadBytes(this Stream? stream, int count)
		{
			byte[] buffer = new byte[count];
			stream.Read(buffer, 0, count);
			return buffer;
		}

		private static int Read7BitEncodedInt(this Stream? stream)
		{
			int count = 0;
			int shift = 0;
			byte b;
			do
			{
				if (shift == 5 * 7) throw new FormatException("Failed to read 7 bit encoding from stream");

				b = (byte)stream.ReadByte();
				count |= (b & 0x7f) << shift;
				shift += 7;
			} while ((b & 0x80) != 0);
			return count;
		}
		
		public static string ReadString(this Stream? stream)
		{
			int length = stream.Read7BitEncodedInt();
			if (length == 0) return string.Empty;

			byte[] buffer = new byte[length];
			stream.Read(buffer, 0, buffer.Length);
			return EzSerializer.Encoding.GetString(buffer);
		}
	}
}
