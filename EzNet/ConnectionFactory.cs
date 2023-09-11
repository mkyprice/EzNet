using EzNet.Transports;
using EzNet.Transports.Tcp;
using EzNet.Transports.Udp;
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
				var udp = new DefaultUdpConnection();
				udp.Bind(0);
				udp.Start();
				transportConnection = new UdpProfile(udp, endPoint);
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
				var udp = new DefaultUdpTransport();
				udp.Bind(endPoint);
				udp.Start();
				transportServer = udp;
			}
			return new Server(transportServer);
		}
	}
}
