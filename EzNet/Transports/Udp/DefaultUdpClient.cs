using EzNet.Transports.Tcp;
using System;
using System.Net;

namespace EzNet.Transports.Udp
{
	public class DefaultUdpClient : ITransportConnection, IDisposable
	{
		public Action<ArraySegment<byte>, ITransportConnection> OnReceive { get; set; }
		public Action<ITransportConnection> OnDisconnect { get; set; }

		internal DefaultUdpClient(DefaultTcpConnection listener, EndPoint endPoint)
		{
			
		}

		public void Send(byte[] bytes)
		{
			
		}
		
		public void Shutdown()
		{
			
		}
		
		public void Dispose()
		{
			
		}
	}
}
