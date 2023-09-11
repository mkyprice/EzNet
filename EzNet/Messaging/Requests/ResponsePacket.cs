﻿using EzNet.Messaging.Extensions;

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
			PacketExtension.Serialize(BaseStream, Packet);
		}
		protected override void Read()
		{
			RequestId = ReadInt();
			Packet = PacketExtension.Deserialize(BaseStream);
		}
	}
}
