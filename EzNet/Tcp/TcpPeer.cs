using EzNet.Messaging;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	internal sealed class TcpPeer : TcpRawConnection
	{
		private readonly TcpServer _server;
		
		public TcpPeer(Socket socket, TcpServer server) : base(socket)
		{
			_server = server;
		}
	}
}
