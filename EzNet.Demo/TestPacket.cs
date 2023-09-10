using EzNet.Messaging;

namespace EzNet.Demo
{
	[Packet("Test")]
	public class TestPacket : BasePacket
	{
		public string Text;
		public double Value;

		public TestPacket() { }

		public TestPacket(string t, double v)
		{
			Text = t;
			Value = v;
		}

		protected override void Write()
		{
			Write(Text);
			Write(Value);
		}
		protected override void Read()
		{
			Text = ReadString();
			Value = ReadDouble();
		}

		public override string ToString()
		{
			return base.ToString() + $"Text: {Text} Float: {Value}";
		}
	}
}
