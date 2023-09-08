using System.Text;

namespace EzNet.Messaging
{
	public class RawPacket : BasePacket
	{
		public int PacketType { get; set; }
		public byte[] Payload { get; protected set; }

		protected override void Write()
		{
			Write(PacketType);
			Write(Payload.Length);
			Write(Payload);
		}
		
		protected override void Read()
		{
			PacketType = ReadInt();
			int length = ReadInt();
			Payload = ReadBytes(length);
		}
	}
}
