namespace EzNet.Messaging
{
	public class ErrorPacket : BasePacket
	{
		public PACKET_ERROR ErrorCode;

		public ErrorPacket() { }
		public ErrorPacket(PACKET_ERROR error)
		{
			ErrorCode = error;
		}

		protected override void Write()
		{
			Write((byte)ErrorCode);
		}
		protected override void Read()
		{
			ErrorCode = (PACKET_ERROR)ReadByte();
		}
	}
}
