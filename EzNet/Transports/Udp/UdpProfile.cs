using System;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Transports.Udp
{
	public class UdpProfile : ITransportConnection
	{
		public Action<ArraySegment<byte>, ITransportConnection> OnReceive { get; set; }
		public Action<ITransportConnection> OnDisconnect { get; set; }

		private readonly EndPoint _endPoint;
		private readonly DefaultUdpConnection _transport;

		public UdpProfile(DefaultUdpConnection transport, EndPoint endPoint)
		{
			_transport = transport;
			_endPoint = endPoint;
			_transport.OnBytesReceived += OnBytesReceived;
		}
		
		private void OnBytesReceived(ArraySegment<byte> segment, EndPoint endPoint)
		{
			if (endPoint == _endPoint)
			{
				OnReceive?.Invoke(segment, this);
			}
		}

		public void Send(byte[] bytes)
		{
			int sent = _transport.SendTo(bytes, SocketFlags.None, _endPoint);
			if (sent <= 0)
			{
				OnDisconnect?.Invoke(this);
			}
		}
		
		public void Shutdown()
		{
			
		}
		
		public void Dispose()
		{
			
		}
	}
}
