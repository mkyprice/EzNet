using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Requests;

namespace EzNet.Tcp
{
	public class TcpClient : TcpPacketConnection
	{
		public TcpClient()
		{
		}

		public void RegisterMessageHandler<TPacket>(Action<TPacket, TcpPacketConnection> callback) 
			where TPacket : BasePacket, new()
		{
			MessageHandler.AddCallback<TPacket>((n) =>
			{
				callback?.Invoke(n.Message, this);
			});
		}
		
		public void RegisterResponseHandler<TRequest, TResponse>(Func<TRequest, TResponse> callback) 
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			RequestHandler.RegisterRequest(callback);
		}
	}
}
