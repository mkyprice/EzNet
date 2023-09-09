using EzNet.Messaging;
using EzNet.Messaging.Requests;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	internal sealed class TcpPeer : TcpPacketConnection
	{
		private readonly TcpServer _server;
		private readonly RequestHandler RequestHandler;
		private Task PacketNotifier;
		
		public TcpPeer(Socket socket, TcpServer server, MessageHandler handler, RequestHandler requestHandler) : base(socket, handler)
		{
			_server = server;
			RequestHandler = requestHandler;
		}

		public async Task<TResponse> SendAsync<TResponse, T>(T packet, int timeoutMS = 2000)
			where T : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			var response = await RequestHandler.SendAsync<T, TResponse>(packet, (p) => this.Send(p), timeoutMS);
			return response;
		}
	}
}
