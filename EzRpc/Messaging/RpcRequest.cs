using EzNet.Messaging;
using EzRpc.Serialization;
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
				Write(Args[i].GetType().AssemblyQualifiedName);
				new EzSerializer().Serialize(BaseStream, Args[i]);
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
				typeStr = ReadString();
				Type argType = Type.GetType(typeStr);
				if (argType != null)
				{
					Args[i] = new EzSerializer().Deserialize(BaseStream, argType);
				}
			}
		}
	}
}
