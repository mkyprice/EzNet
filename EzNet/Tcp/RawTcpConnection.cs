using EzNet.Logging;
using EzNet.Messaging;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	public abstract class RawTcpConnection : IDisposable
	{
		public bool IsConnected => _connection.Connected;
		
		protected bool IsDisposed { get; private set; }

		private static readonly int MEGABYTE = 1048576;
		private static readonly int BUFFER_SIZE = MEGABYTE;
		
		private readonly Socket _connection;
		private readonly byte[] _receiveBuffer = new byte[BUFFER_SIZE];

		public RawTcpConnection()
		{
			PacketSerializerExtension.Init();
			_connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public RawTcpConnection(Socket socket)
		{
			_connection = socket;
			
			if (_connection.Connected)
			{
				_connection.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnBytesReceived, _connection);
			}
		}
		
		public bool Send(byte[] bytes)
		{
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

		#region Virtual overloads

		protected virtual void OnShutdown() { }

		#endregion

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
			if (received == 0)
			{
				Log.Info("Socket received zero bytes. Shutting down");
				Shutdown();
				return;
			}
			Log.Debug("{0} received {1} bytes", this, received);
			Receive(new ArraySegment<byte>(_receiveBuffer, 0, received));
			_connection.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, OnBytesReceived, _connection);
		}

		protected abstract void Receive(ArraySegment<byte> bytes);
		
		private void Shutdown()
		{
			if (_connection.Connected == false)
			{
				return;
			}
			_connection.Shutdown(SocketShutdown.Both);
			OnShutdown();
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
