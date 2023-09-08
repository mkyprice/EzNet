using System.Reflection;
using System.Text;

namespace EzRPC.ClientDemo
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
			       Float == other.Float &&
			       Double == other.Double &&
			       Decimal == other.Decimal &&
			       Bool == other.Bool &&
			       String == other.String &&
			       DateTime == other.DateTime;

		}
	}
}
