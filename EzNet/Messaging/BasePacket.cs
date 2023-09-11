﻿using EzNet.IO;
using EzNet.IO.Extensions;
using System;
using System.IO;
using System.Text;

namespace EzNet.Messaging
{
	public abstract class BasePacket : IDisposable
	{
		public static Encoding Encoding = Encoding.UTF8;
		
		protected Stream? BaseStream;

		protected abstract void Write();
		protected abstract void Read();

		public void Write(Stream? stream)
		{
			BaseStream = stream;
			Write();
			BaseStream = null;
		}

		public void Read(Stream? stream)
		{
			BaseStream = stream;
			Read();
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
