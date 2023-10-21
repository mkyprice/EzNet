// using EzNet.Messaging.Extensions;
// using System;
//
// namespace EzNet.Messaging.Requests
// {
// 	internal class ResponsePacket : BasePacket
// 	{
// 		public int RequestId;
// 		public BasePacket Response;
//
// 		public ResponsePacket() { }
// 		public ResponsePacket(BasePacket response, int requestId)
// 		{
// 			Response = response;
// 			RequestId = requestId;
// 		}
//
// 		protected override void Write()
// 		{
// 			Write(RequestId);
// 			PacketExtension.Serialize(BaseStream, Response);
// 		}
// 		
// 		protected override void Read()
// 		{
// 			RequestId = ReadInt();
// 			Response = PacketExtension.Deserialize(BaseStream);
// 		}
// 	}
// }
