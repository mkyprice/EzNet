using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Requests;
using System;
using System.Net;

namespace EzNet.Tcp
{
	public class TcpClient : PacketConnection
	{
		public TcpClient(IConnection connection) : base(connection)
		{
		}

		public TcpClient(EndPoint endPoint) : base(new DefaultTcpConnection(endPoint))
		{
		}

		public void RegisterMessageHandler<TPacket>(Action<TPacket, PacketConnection> callback) 
			where TPacket : BasePacket, new()
		{
			MessageHandler.AddCallback<TPacket>((n) =>
			{
				callback?.Invoke(n.Message, n.Source);
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
