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
		private readonly UdpConnection _transport;
		private bool IsAlive = true;

		public UdpProfile(UdpConnection transport, EndPoint endPoint)
		{
			_transport = transport;
			_endPoint = endPoint;
			_transport.OnBytesReceived += OnBytesReceived;
		}
		
		private void OnBytesReceived(ArraySegment<byte> segment, EndPoint endPoint)
		{
			if (endPoint.Equals(_endPoint))
			{
				OnReceive?.Invoke(segment, this);
			}
		}

		public bool Send(byte[] bytes)
		{
			int sent = _transport.SendTo(bytes, SocketFlags.None, _endPoint);
			if (sent <= 0)
			{
				OnDisconnect?.Invoke(this);
				return false;
			}
			return true;
		}
		
		public void Shutdown()
		{
			if (IsAlive == false) return;
			IsAlive = false;
			OnDisconnect?.Invoke(this);
			OnReceive = null;
			OnDisconnect = null;
		}
		
		public void Dispose()
		{
			Shutdown();
		}
	}
}
