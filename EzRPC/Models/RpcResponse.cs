using EzNet.Messaging;
using EzRPC.Logging;
using EzRPC.Serialization;
using System;
using System.IO;

namespace EzRPC
{
	public class RpcResponse : BasePacket
	{
		public RpcResponse() { }
		
		public RpcResponse(object result, RPC_ERROR error)
		{
			Result = result;
			Error = error;
		}
		
		/// <summary>
		/// Result of the method call
		/// Null if error or void
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// Error message
		/// </summary>
		public RPC_ERROR Error { get; set; }
		protected override void Write()
		{
			using (var serializer = new EzSerializer())
			{
				Write(Result?.GetType().AssemblyQualifiedName ?? string.Empty);
				if (Result != null)
				{
					serializer.Serialize(BaseStream, Result);
				}
			}
			Write((byte)Error);
		}
		protected override void Read()
		{
			using (var serializer = new EzSerializer())
			{
				string t = ReadString();
				if (string.IsNullOrEmpty(t) == false)
				{
					Type type = Type.GetType(t);
					if (type != null)
					{
						Result = serializer.Deserialize(BaseStream, type);
					}
					else
					{
						Log.Warn("Failed to parse type from {0}", t);
					}
				}
			}
			Error = (RPC_ERROR)ReadByte();
		}
	}
}
