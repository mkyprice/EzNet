namespace EzNet.Messaging.Requests
{
	internal class ResponsePacket : BasePacket
	{
		public int RequestId;
		public BasePacket Packet;

		public ResponsePacket() { }
		public ResponsePacket(BasePacket packet, int requestId)
		{
			Packet = packet;
			RequestId = requestId;
		}

		protected override void Write()
		{
			Write(RequestId);
			byte[] bytes = PacketSerializerExtension.Serialize(Packet);
			Write(bytes);
		}
		protected override void Read()
		{
			RequestId = ReadInt();
			Packet = PacketSerializerExtension.Deserialize(_baseStream);
		}
	}
}
