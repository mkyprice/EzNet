using EzNet.Messaging;
using System.Text;

namespace EzNet.Demo
{
	public class DemoPacket : BasePacket
	{
		public string Text { get; set; }

		public DemoPacket()
		{
			
		}

		public DemoPacket(string text) : base()
		{
			Text = text;
		}

		public override string ToString()
		{
			return Text;
		}
		protected override void Write()
		{
			Write(Text);
		}
		protected override void Read()
		{
			Text = ReadString();
		}
	}
}
