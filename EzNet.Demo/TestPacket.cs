using EzNet.Messaging;

namespace EzNet.Demo
{
	public class TestPacket : BasePacket
	{
		public string Text;
		public float Float;

		public TestPacket() { }

		public TestPacket(string t, float v)
		{
			Text = t;
			Float = v;
		}

		protected override void Write()
		{
			Write(Text);
			Write(Float);
		}
		protected override void Read()
		{
			Text = ReadString();
			Float = ReadSingle();
		}

		public override string ToString()
		{
			return base.ToString() + $"Text: {Text} Float: {Float}";
		}
	}
}
