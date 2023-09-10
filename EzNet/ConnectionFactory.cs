using EzNet.Transports;
using EzNet.Transports.Tcp;
using System;
using System.Net;

namespace EzNet
{
	public static class ConnectionFactory
	{
		public static Connection BuildClient(EndPoint endPoint, bool reliable)
		{
			ITransportConnection transportConnection;
			if (reliable)
			{
				transportConnection = new DefaultTcpConnection(endPoint);
			}
			else
			{
				throw new NotImplementedException("Unreliable connections are not yet implemented");
			}
			return new Connection(transportConnection);
		}
		
		public static Server BuildServer(EndPoint endPoint, bool reliable)
		{
			ITransportServer transportServer;
			if (reliable)
			{
				var tcpServer = new DefaultTcpServer();
				tcpServer.Listen(endPoint);
				transportServer = tcpServer;
			}
			else
			{
				throw new NotImplementedException("Unreliable connections are not yet implemented");
			}
			return new Server(transportServer);
		}
	}
}
