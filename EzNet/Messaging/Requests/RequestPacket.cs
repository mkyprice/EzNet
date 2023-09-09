namespace EzNet.Messaging.Requests
{
	internal class RequestPacket : BasePacket
	{
		public int RequestId = Guid.NewGuid().GetHashCode();
		public BasePacket Packet;

		public RequestPacket() { }
		public RequestPacket(BasePacket packet)
		{
			Packet = packet;
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
