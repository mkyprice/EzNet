using EzNet.IO;
using EzNet.IO.Extensions;
using EzNet.Logging;
using System.Text;

namespace EzNet.Messaging
{
	public abstract class BasePacket : IDisposable
	{
		public static Encoding Encoding = Encoding.UTF8;
		
		protected Stream _baseStream;

		protected abstract void Write();
		protected abstract void Read();

		public void Write(Stream stream)
		{
			_baseStream = stream;
			Write();
			_baseStream = null;
		}

		public void Read(Stream stream)
		{
			_baseStream = stream;
			Read();
			_baseStream = null;
		}
		

		#region Writing
		
		protected void Write(byte value) => _baseStream.Write(value);
		protected void Write(sbyte value) => _baseStream.Write(value);
		protected void Write(short value) => _baseStream.Write(value);
		protected void Write(ushort value) => _baseStream.Write(value);
		protected void Write(int value) => _baseStream.Write(value);
		protected void Write(uint value) => _baseStream.Write(value);
		protected void Write(long value) => _baseStream.Write(value);
		protected void Write(ulong value) => _baseStream.Write(value);
		protected void Write(float value) => _baseStream.Write(value);
		protected void Write(double value) => _baseStream.Write(value);
		protected void Write(byte[] value) => _baseStream.Write(value);
		protected void Write(char value) => _baseStream.Write(value);
		protected void Write(string value) => _baseStream.Write(value ?? string.Empty);

		#endregion

		#region Reading


		protected byte ReadByte() => (byte)_baseStream.ReadByte();
		protected sbyte ReadSByte() => _baseStream.ReadSByte();
		protected short ReadShort() => _baseStream.ReadShort();
		protected ushort ReadUShort() => _baseStream.ReadUShort();
		protected int ReadInt() => _baseStream.ReadInt();
		protected uint ReadUInt() => _baseStream.ReadUInt();
		protected long ReadLong() => _baseStream.ReadLong();
		protected ulong ReadULong() => _baseStream.ReadULong();
		protected float ReadSingle() => _baseStream.ReadSingle();
		protected double ReadDouble() => _baseStream.ReadDouble();
		protected byte[] ReadBytes(int count) => _baseStream.ReadBytes(count);
		protected char ReadChar() => _baseStream.ReadChar();
		protected string ReadString() => _baseStream.ReadString();

		#endregion

		public void Dispose()
		{
			_baseStream.Dispose();
		}
	}
}
