using EzNet.Logging;
using EzNet.Transports.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Transports.Udp
{
	public class DefaultUdpConnection : IDisposable
	{
		public bool Alive => _isShutdown == false;
		public Action<ArraySegment<byte>, EndPoint> OnBytesReceived;

		private const int MEGABYTE = 1048576;
		private const int BUFFER_SIZE = MEGABYTE;

		private readonly Socket _socket;
		private byte[] _buffer = new byte[BUFFER_SIZE];
		private bool _isShutdown = true;

		private readonly Dictionary<EndPoint, DefaultUdpTransport> _clients = new Dictionary<EndPoint, DefaultUdpTransport>();
		
		public DefaultUdpConnection()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_socket.ReceiveBufferSize = BUFFER_SIZE;
			_socket.SendBufferSize = BUFFER_SIZE;
			_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		}

		public void Bind(EndPoint endPoint)
		{
			if (_isShutdown)
			{
				_socket.Bind(endPoint);
			}
			else
			{
				Log.Warn("Tried to call Bind on an active connection");
			}
		}

		public void Bind(int port)
		{
			EndPoint endPoint = SocketExtensions.GetEndPoint(port, AddressFamily.InterNetwork);
			Bind(endPoint);
		}

		public void Start()
		{
			if (_isShutdown == false)
			{
				Console.WriteLine("WARNING: Tried to call start on an active connection");
				return;
			}
			if (_socket.IsBound == false)
			{
				Bind(0);
			}
			_isShutdown = false;

			EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
			_socket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, OnReceiveFrom, _socket);
		}

		private void OnReceiveFrom(IAsyncResult result)
		{
			EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
			Socket socket = (Socket)result.AsyncState;

			int received = socket.EndReceiveFrom(result, ref ep);
			if (received == 0)
			{
				Log.Info("No bytes received. Shutting down");
				ShutDown();
				return;
			}

			OnBytesReceived?.Invoke(new ArraySegment<byte>(_buffer, 0, received), ep);
			_socket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref ep, OnReceiveFrom, _socket);
		}

		public int SendTo(byte[] payload, SocketFlags flags, EndPoint endPoint)
		{
			try
			{
				return _socket.SendTo(payload, flags, endPoint);
			}
			catch (Exception e)
			{
				Log.Warn(e.Message);
			}
			return 0;
		}

		public void ShutDown()
		{
			if (_isShutdown) return;
			
			_isShutdown = true;
		}
		
		public void Dispose()
		{
			ShutDown();
			_socket.Dispose();
		}
	}
}
