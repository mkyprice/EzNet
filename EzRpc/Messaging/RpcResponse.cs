using EzNet.Messaging;
using EzRpc.Serialization;
using EzRpc.Serialization.Extensions;
using System;

namespace EzRpc.Messaging
{
	[Packet("RpcRes")]
	public class RpcResponse : BasePacket
	{
		public object? Result;
		public RPC_ERROR Error;
		protected override void Write()
		{
			Write((byte)Error);
			BaseStream.Serialize(Result);
		}
		protected override void Read()
		{
			Error = (RPC_ERROR)ReadByte();
			Result = BaseStream.Deserialize();
		}
	}
}
