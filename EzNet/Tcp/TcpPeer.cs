using EzNet.Messaging;
using EzNet.Messaging.Requests;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	internal sealed class TcpPeer : TcpPacketConnection
	{
		private readonly TcpServer _server;
		private Task PacketNotifier;
		
		public TcpPeer(Socket socket, TcpServer server, MessageHandler handler, RequestHandler requestHandler) : base(socket, handler, requestHandler)
		{
			_server = server;
		}
	}
}
