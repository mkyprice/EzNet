using EzNet.Transports.Tcp;
using System;
using System.Net;

namespace EzNet
{
	/// <summary>
	/// TODO: Create connections through the factory
	/// </summary>
	public static class ConnectionFactory
	{
		public static Connection BuildClient(EndPoint endPoint, bool reliable)
		{
			Connection connection;
			if (reliable)
			{
				connection = new Connection(new DefaultTcpConnection(endPoint));
			}
			else
			{
				connection = default;
			}
			return connection;
		}
		
		public static Server BuildServer(EndPoint endPoint, bool reliable)
		{
			Server server;
			if (reliable)
			{
				var tcpServer = new DefaultTcpServer();
				tcpServer.Listen(endPoint);
				server = new Server(tcpServer);
			}
			else
			{
				throw new NotImplementedException("Unreliable connections are not yet implemented");
			}
			return server;
		}
	}
}
