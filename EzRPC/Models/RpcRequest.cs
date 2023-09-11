using EzNet.Messaging;
using EzRPC.Reflection.Extensions;
using EzRPC.Serialization;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcRequest : BasePacket
	{
		public string MethodName { get; set; }
		public Type DeclaringType { get; set; }
		public object[] Params { get; set; }
		
		
		protected override void Write()
		{
			Write(MethodName);
			Write(DeclaringType.AssemblyQualifiedName);
			Write((byte)(Params?.Length ?? 0));
			using var serializer = new EzSerializer();
			for (int i = 0; i < Params?.Length; i++)
			{
				Write(Params[i].GetType().AssemblyQualifiedName);
				serializer.Serialize(BaseStream, Params[i]);
			}
		}
		
		protected override void Read()
		{
			MethodName = ReadString();
			DeclaringType = Type.GetType(ReadString());
			int paramLen = ReadByte();
			Params = new object[paramLen];
			using var serializer = new EzSerializer();
			for (int i = 0; i < paramLen; i++)
			{
				Type type = Type.GetType(ReadString());
				Params[i] = serializer.Deserialize(BaseStream, type);
			}
		}
	}
}
