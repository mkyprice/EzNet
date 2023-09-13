using EzNet.Messaging;
using System.Reflection;
using System.Text;

namespace EzNet.Demo
{
	public class TestValueClass
	{
		public byte Byte;
		public sbyte Sbyte;
		public short Short;
		public ushort UShort;
		public int Int;
		public uint UInt;
		public long Long;
		public ulong ULong;
		public char Char;
		public float Float;
		public double Double;
		public decimal Decimal;
		public bool Bool;
		public string String;
		public DateTime DateTime;

		public TestValueClass()
		{
			Byte = (byte)Random.Shared.Next(byte.MinValue, byte.MaxValue);
			Sbyte = (sbyte)Random.Shared.Next(sbyte.MinValue, sbyte.MaxValue);
			Short = (short)Random.Shared.Next(short.MinValue, short.MaxValue);
			UShort = (ushort)Random.Shared.Next(ushort.MinValue, ushort.MaxValue);
			Int = (int)Random.Shared.Next(int.MinValue, int.MaxValue);
			UInt = (uint)Random.Shared.Next(0, int.MaxValue);
			Long = (long)Random.Shared.Next(int.MinValue, int.MaxValue);
			ULong = (ulong)Random.Shared.Next(int.MinValue, int.MaxValue);
			Char = (char)Random.Shared.Next(0, 255);
			Float = Random.Shared.NextSingle();
			Double = Random.Shared.NextDouble();
			Decimal = (decimal)Random.Shared.NextSingle();
			Bool = Random.Shared.Next(1) == 1;
			String = Guid.NewGuid().ToString();
			DateTime = DateTime.Now;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (FieldInfo field in GetType().GetFields(BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.Public | 
			                                                BindingFlags.NonPublic))
			{
				sb.AppendFormat("{0}: {1} ", field.FieldType.Name, field.GetValue(this));
			}
			return sb.ToString();
		}

		public override bool Equals(object? obj)
		{
			return obj is TestValueClass other &&
			       Byte == other.Byte &&
			       Sbyte == other.Sbyte &&
			       Short == other.Short &&
			       UShort == other.UShort &&
			       Int == other.Int &&
			       UInt == other.UInt &&
			       Long == other.Long &&
			       ULong == other.ULong &&
			       Char == other.Char &&
			       Math.Abs(Float - other.Float) < 0.001f &&
			       Decimal == other.Decimal &&
			       Math.Abs(Double - other.Double) < 0.001f &&
			       Bool == other.Bool &&
			       String == other.String &&
			       DateTime == other.DateTime;

		}
		// protected override void Write()
		// {
		// 	Write(Byte);
		// 	Write(Sbyte);
		// 	Write(Short);
		// 	Write(UShort);
		// 	Write(Int);
		// 	Write(UInt);
		// 	Write(Long);
		// 	Write(ULong);
		// 	Write(Char);
		// 	Write(Float);
		// 	Write(Decimal);
		// 	Write(Double);
		// 	Write(Bool);
		// 	Write(String);
		// 	Write(DateTime.ToBinary());
		// }
		// protected override void Read()
		// {
		// 	Byte = ReadByte();
		// 	Sbyte = ReadSByte();
		// 	Short = ReadShort();
		// 	UShort = ReadUShort();
		// 	Int = ReadInt();
		// 	UInt = ReadUInt();
		// 	Long = ReadLong();
		// 	ULong = ReadULong();
		// 	Char = ReadChar();
		// 	Float = ReadSingle();
		// 	Decimal = ReadDecimal();
		// 	Double = ReadDouble();
		// 	Bool = ReadBool();
		// 	String = ReadString();
		// 	DateTime = DateTime.FromBinary(ReadLong());
		// }
	}
}
