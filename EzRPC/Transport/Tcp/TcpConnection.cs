using EzRPC.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EzRPC.Transport.Tcp
{
	internal class TcpConnection : ITcpConnection, IDisposable
	{
		private readonly Socket _connection;
		private const int MEGABYTE = 1048576;
		private const int BUFFER_SIZE = MEGABYTE;
		private readonly byte[] _buffer = new byte[BUFFER_SIZE];

		public TcpConnection()
		{
			_connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public bool Send(byte[] bytes)
		{
			_connection.Send(bytes, 0, bytes.Length, SocketFlags.None, out SocketError error);
			if (error != SocketError.Success)
			{
				Log.Warn("{0} Failed to send {1} bytes. Reason: {2}", this, bytes.Length, error);
			}
			return error != SocketError.Success;
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

				_connection.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnBytesReceived, _connection);
			}
			return _connection.Connected;
		}

		private void OnBytesReceived(IAsyncResult result)
		{
			Socket socket = result.AsyncState as Socket;

			int received = socket.EndReceive(result, out SocketError error);
			if (error != SocketError.Success)
			{
				Log.Warn("Socket encountered error {0}", error);
				Shutdown();
				return;
			}
			Log.Info("Received {0} bytes", received);
			_connection.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnBytesReceived, _connection);
		}

		private void Shutdown()
		{
			if (_connection.Connected == false)
			{
				Log.Warn("Called shutdown on an unconnected socket");
				return;
			}
			_connection.Shutdown(SocketShutdown.Both);
		}
		
		public virtual void Dispose()
		{
			Shutdown();
			_connection.Dispose();
		}
	}
}
