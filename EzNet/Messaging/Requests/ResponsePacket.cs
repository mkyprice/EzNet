using EzNet.Messaging.Extensions;
using EzNet.Serialization;
using System;

namespace EzNet.Messaging.Requests
{
	internal class ResponsePacket : BasePacket
	{
		public int RequestId;
		public object Response;

		public ResponsePacket() { }
		public ResponsePacket(object response, int requestId)
		{
			Response = response;
			RequestId = requestId;
		}

		protected override void Write()
		{
			Write(RequestId);
			if (Response != null)
			{
				// TODO: Find a better way to serialize types
				string responseName = Response.GetType().AssemblyQualifiedName;
				Write(responseName);
				var serializer = new XmlByteSerializer();
				serializer.Serialize(BaseStream, Response);
			}
			else
			{
				Write(string.Empty);
			}
		}
		
		protected override void Read()
		{
			RequestId = ReadInt();
			string t = ReadString();
			if (string.IsNullOrEmpty(t) == false)
			{
				Type type = Type.GetType(t);
				var serializer = new XmlByteSerializer();
				Response = serializer.Deserialize(BaseStream, type);
			}
		}
	}
}
