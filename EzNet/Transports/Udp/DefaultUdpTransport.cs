using EzNet.Transports.Tcp;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Transports.Udp
{
	public class DefaultUdpTransport : ITransportConnection, ITransportServer, IDisposable
	{
		public Action<ITransportConnection> OnNewConnection { get; set; }
		public Action<ITransportConnection> OnEndConnection { get; set; }
		public Action<ArraySegment<byte>, ITransportConnection> OnReceive { get; set; }
		public Action<ITransportConnection> OnDisconnect { get; set; }

		private readonly DefaultUdpConnection _udp = new DefaultUdpConnection();

		private readonly ConcurrentDictionary<EndPoint, ITransportConnection> _connections = new ConcurrentDictionary<EndPoint, ITransportConnection>();

		internal DefaultUdpTransport()
		{
			_udp.OnBytesReceived += OnBytesReceived;
		}
		internal DefaultUdpTransport(EndPoint endPoint)
		{
			_udp.OnBytesReceived += OnBytesReceived;
			_connections[endPoint] = new UdpProfile(_udp, endPoint);
		}

		public void Bind(EndPoint endPoint)
		{
			_udp.Bind(endPoint);
		}

		public void Bind(int port)
		{
			_udp.Bind(port);
		}

		public void Start()
		{
			_udp.Start();
		}
		
		private void OnBytesReceived(ArraySegment<byte> bytes, EndPoint endPoint)
		{
			if (_connections.ContainsKey(endPoint) == false)
			{
				_connections[endPoint] = new UdpProfile(_udp, endPoint);
				OnNewConnection?.Invoke(_connections[endPoint]);
				OnReceive += _connections[endPoint].OnReceive;
				OnReceive?.Invoke(bytes, _connections[endPoint]);
			}
		}
		
		public bool Send(byte[] bytes)
		{
			foreach (var connection in _connections.Values)
			{
				connection.Send(bytes);
			}
			return true;
		}
		
		public void Shutdown()
		{
			
		}
		
		public void Dispose()
		{
			
		}
	}
}
