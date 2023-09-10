﻿using EzNet.Messaging;

namespace EzNet.Demo
{
	[Packet("Test")]
	public class TestPacket : BasePacket
	{
		public string Text;
		public double Float;

		public TestPacket() { }

		public TestPacket(string t, double v)
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
			Float = ReadDouble();
		}

		public override string ToString()
		{
			return base.ToString() + $"Text: {Text} Float: {Float}";
		}
	}
}
