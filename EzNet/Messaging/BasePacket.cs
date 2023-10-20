using EzNet.IO;
using EzNet.IO.Extensions;
using EzNet.Logging;
using System;
using System.IO;
using System.Text;

namespace EzNet.Messaging
{
	public abstract class BasePacket : IDisposable
	{
		public static Encoding Encoding = Encoding.UTF8;
		public PACKET_ERROR Error { get; set; }
		protected Stream? BaseStream;

		protected abstract void Write();
		protected abstract void Read();

		public int PeekLength(Stream stream)
		{
			short len = stream.ReadShort();
			stream.Position -= sizeof(short);
			return len;
		}

		public void Write(Stream stream)
		{
			BaseStream = stream;
			long pos = BaseStream.Position;
			// Save room for packet length
			BaseStream.Position += sizeof(uint);
			// Write Error message
			BaseStream.Write((byte)Error);
			// Write packet data
			Write();
			// Write packet length
			long curr = BaseStream.Position;
			BaseStream.Position = pos;
			Write((uint)(curr - pos));
			BaseStream.Position = curr;
			BaseStream = null;
		}

		public void Read(Stream stream)
		{
			BaseStream = stream;
			long pos = BaseStream.Position;
			uint length = ReadUInt();
			long finalPos = pos + length;
			Error = (PACKET_ERROR)ReadByte();
			Read();
			if (BaseStream.Position != finalPos)
			{
				Log.Warn("Misread packet {0}. Read {1} bytes. Should be {2}", 
					this, BaseStream.Position - pos, length);
				BaseStream.Position = finalPos;
			}
			BaseStream = null;
		}
		

		#region Writing
		
		protected void Write(bool value) => BaseStream.Write(value);
		protected void Write(byte value) => BaseStream.Write(value);
		protected void Write(sbyte value) => BaseStream.Write(value);
		protected void Write(short value) => BaseStream.Write(value);
		protected void Write(ushort value) => BaseStream.Write(value);
		protected void Write(int value) => BaseStream.Write(value);
		protected void Write(uint value) => BaseStream.Write(value);
		protected void Write(long value) => BaseStream.Write(value);
		protected void Write(ulong value) => BaseStream.Write(value);
		protected void Write(float value) => BaseStream.Write(value);
		protected void Write(decimal value) => BaseStream.Write(value);
		protected void Write(double value) => BaseStream.Write(value);
		protected void Write(byte[] value) => BaseStream.Write(value, 0, value.Length);
		protected void Write(char value) => BaseStream.Write(value);
		protected void Write(string value) => BaseStream.Write(value ?? string.Empty);

		#endregion

		#region Reading


		protected bool ReadBool() => BaseStream.ReadBool();
		protected byte ReadByte() => (byte)BaseStream.ReadByte();
		protected sbyte ReadSByte() => BaseStream.ReadSByte();
		protected short ReadShort() => BaseStream.ReadShort();
		protected ushort ReadUShort() => BaseStream.ReadUShort();
		protected int ReadInt() => BaseStream.ReadInt();
		protected uint ReadUInt() => BaseStream.ReadUInt();
		protected long ReadLong() => BaseStream.ReadLong();
		protected ulong ReadULong() => BaseStream.ReadULong();
		protected float ReadSingle() => BaseStream.ReadSingle();
		protected decimal ReadDecimal() => BaseStream.ReadDecimal();
		protected double ReadDouble() => BaseStream.ReadDouble();
		protected byte[] ReadBytes(int count) => BaseStream.ReadBytes(count);
		protected char ReadChar() => BaseStream.ReadChar();
		protected string ReadString() => BaseStream.ReadString();

		#endregion

		public void Dispose()
		{
			BaseStream.Dispose();
		}
	}
}
