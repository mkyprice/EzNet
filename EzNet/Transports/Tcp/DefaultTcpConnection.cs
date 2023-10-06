using EzNet.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Transports.Tcp
{
	public class DefaultTcpConnection : ITransportConnection, IDisposable
	{
		public bool IsConnected => _connection.Connected;
		public Action<ArraySegment<byte>, ITransportConnection> OnReceive { get; set; }
		public Action<ITransportConnection> OnDisconnect { get; set; }

		protected bool IsDisposed { get; private set; }

		private static readonly int MEGABYTE = 1048576;
		private static readonly int BUFFER_SIZE = MEGABYTE;
		
		private readonly Socket _connection;
		private readonly byte[] _receiveBuffer = new byte[BUFFER_SIZE];

		public DefaultTcpConnection()
		{
			_connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public DefaultTcpConnection(Socket socket)
		{
			_connection = socket;
			
			if (_connection.Connected)
			{
				_connection.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnBytesReceived, _connection);
			}
		}

		public DefaultTcpConnection(EndPoint endPoint) : this()
		{
			Connect(endPoint);
		}

		bool ITransportConnection.IsConnected()
		{
			return IsConnected;
		}
		public bool Send(byte[] bytes)
		{
			if (_connection.Connected == false)
			{
				Log.Warn("Tried to send {0} bytes with unconnected socket", bytes.Length);
				return false;
			}
			int totalSent = 0;
			while (totalSent < bytes.Length)
			{
				int sent = _connection.Send(bytes, totalSent, bytes.Length - totalSent, SocketFlags.None, out SocketError error);
				if (error != SocketError.Success || sent <= 0)
				{
					Log.Warn("{0} Failed to send {1} bytes. Reason: {2}", this, bytes.Length, error);
					Shutdown();
					return false;
				}
				totalSent += sent;
			}
			return true;
		}
		
		public bool Connect(EndPoint endPoint)
		{
			if (_connection.Connected)
			{
				Log.Warn("EzTcpClient tried to connect to {0} but is already connected to {1}", 
					endPoint, _connection.RemoteEndPoint);
				return false;
			}

			try
			{
				_connection.Connect(endPoint);
			}
			catch (SocketException e)
			{
				Log.Warn("Failed to connect {0}", e.Message);
			}
			
			if (_connection.Connected)
			{
				Log.Info("Connected to {0}", endPoint);

				_connection.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnBytesReceived, _connection);
			}
			return _connection.Connected;
		}

		private void OnBytesReceived(IAsyncResult result)
		{
			Socket socket = result.AsyncState as Socket;
			if (socket.Connected == false) return;

			int received = socket.EndReceive(result, out SocketError error);
			if (error != SocketError.Success || received == 0)
			{
				string message = received == 0 ? "Socket received zero bytes. Shutting down" : string.Format("Socket encountered error {0}", error);
				Log.Warn(message);
				Shutdown();
				return;
			}
			Log.Debug("{0} received {1} bytes", this, received);
			OnReceive?.Invoke(new ArraySegment<byte>(_receiveBuffer, 0, received), this);
			_connection.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnBytesReceived, _connection);
		}
		
		public void Shutdown()
		{
			if (_connection.Connected == false)
			{
				return;
			}
			_connection.Shutdown(SocketShutdown.Both);
			OnDisconnect?.Invoke(this);
			Dispose();
		}
		
		public virtual void Dispose()
		{
			if (IsDisposed) return;
			
			IsDisposed = true;
			Shutdown();
			_connection.Dispose();
		}
	}
}
