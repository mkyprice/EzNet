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
			PacketSerializerExtension.Serialize(_baseStream, Packet);
		}
		protected override void Read()
		{
			RequestId = ReadInt();
			Packet = PacketSerializerExtension.Deserialize(_baseStream);
		}
	}
}
