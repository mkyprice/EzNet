using EzNet.Messaging;
using EzRpc.Serialization;
using EzRpc.Serialization.Extensions;
using System;

namespace EzRpc.Messaging
{
	[Packet("RpcReq")]
	public class RpcRequest : BasePacket
	{
		public Type Type;
		public string Method;
		public object[] Args;
		
		protected override void Write()
		{
			Write(Type.AssemblyQualifiedName);
			Write(Method);
			Write(Args?.Length ?? 0);
			for (int i = 0; i < Args?.Length; i++)
			{
				BaseStream.Serialize(Args[i]);
			}
		}
		protected override void Read()
		{
			string typeStr = ReadString();
			Type = Type.GetType(typeStr);
			Method = ReadString();
			int len = ReadInt();
			Args = new object[len];
			for (int i = 0; i < Args.Length; i++)
			{
				Args[i] = BaseStream.Deserialize();
			}
		}
	}
}
