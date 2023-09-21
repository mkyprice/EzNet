using EzNet.Messaging;
using EzRpc.Serialization;
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
			Write(Result?.GetType().AssemblyQualifiedName ?? string.Empty);
			new EzSerializer().Serialize(BaseStream, Result);
		}
		protected override void Read()
		{
			Error = (RPC_ERROR)ReadByte();
			string typeStr = ReadString();
			Type argType = Type.GetType(typeStr);
			if (argType != null)
			{
				Result = new EzSerializer().Deserialize(BaseStream, argType);
			}
		}
	}
}
