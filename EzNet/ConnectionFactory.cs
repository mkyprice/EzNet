using EzNet.Transports;
using EzNet.Transports.Tcp;
using EzNet.Transports.Udp;
using System.Net;

namespace EzNet
{
	public static class ConnectionFactory
	{
		#region Client building
		
		/// <summary>
		/// Build a client using an external transport system
		/// </summary>
		/// <param name="transport"></param>
		/// <returns></returns>
		public static Connection BuildClient(ITransportConnection transport)
		{
			Connection connection = new Connection(transport);
			return connection;
		}
		
		/// <summary>
		/// Build a client connection
		/// </summary>
		/// <param name="endPoint"></param>
		/// <param name="reliable"></param>
		/// <returns></returns>
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
			return BuildClient(transportConnection);
		}

		#endregion


		#region Server building
		
		/// <summary>
		/// Build a server using a custom transport server
		/// </summary>
		/// <param name="transport"></param>
		/// <returns></returns>
		public static Server BuildServer(ITransportServer transport)
		{
			Server server = new Server(transport);
			return server;
		}

		public static Server BuildServer(int port, bool reliable)
		{
			ITransportServer transportServer;
			if (reliable)
			{
				var tcpServer = new DefaultTcpServer();
				tcpServer.Listen(port);
				transportServer = tcpServer;
			}
			else
			{
				var udp = new DefaultUdpTransport();
				udp.Bind(port);
				udp.Start();
				transportServer = udp;
			}
			return BuildServer(transportServer);
		}
		
		/// <summary>
		/// Build a server
		/// </summary>
		/// <param name="endPoint"></param>
		/// <param name="reliable"></param>
		/// <returns></returns>
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
			return BuildServer(transportServer);
		}

		#endregion
	}
}
