using EzRPC.Logging;
using System.Collections.Generic;

namespace EzRPC.Transport
{
	public class NetworkPacketQueue : INetworkReceiver
	{
		private readonly Queue<RpcRequest> _requests = new Queue<RpcRequest>();
		private readonly Queue<RpcResponse> _responses = new Queue<RpcResponse>();
		
		public void ReceiveBytes(byte[] bytes)
		{
			RPC_MESSAGE_TYPE packet_type = (RPC_MESSAGE_TYPE)bytes[0];
			switch (packet_type)
			{
				case RPC_MESSAGE_TYPE.Request:
					
					break;
				case RPC_MESSAGE_TYPE.Response:
					
					break;
				default:
					Log.Warn("Unregonized packet type {0}", bytes[0]);
					break;
			}
		}
	}
}
