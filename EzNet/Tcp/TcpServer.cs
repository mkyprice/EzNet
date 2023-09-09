using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Requests;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	public class TcpServer : IDisposable
	{
		public readonly MessageHandler MessageHandler = new MessageHandler();
		protected readonly RequestHandler RequestHandler;
		private readonly List<TcpPeer> _connections = new List<TcpPeer>();
		private readonly Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		public TcpServer()
		{
			RequestHandler = new RequestHandler(MessageHandler);
		}

		public void Listen(EndPoint endPoint)
		{
			Listener.Bind(endPoint);
			Listener.Listen(100);

			Listener.BeginAccept(OnConnection, Listener);
		}
		
		public void RegisterResponseHandler<TPacket, TResponse>(Func<TPacket, TResponse> callback) 
			where TPacket : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			RequestHandler.RegisterRequest(callback);
		}

		public void Broadcast<T>(T packet)
			where T : BasePacket
		{
			foreach (TcpPeer connection in _connections)
			{
				connection.Send(packet); // TODO: send raw byte[] method
			}
		}

		private void OnConnection(IAsyncResult result)
		{
			Socket socket = ((Socket)result.AsyncState).EndAccept(result);
			TcpPeer connection = new TcpPeer(socket, this, MessageHandler, RequestHandler);
			_connections.Add(connection);
			
			Listener.BeginAccept(OnConnection, Listener);
		}

		private void Shutdown()
		{
			if (Listener.Connected == false)
			{
				return;
			}
			Listener.Shutdown(SocketShutdown.Both);
		}
		
		public void Dispose()
		{
			foreach (RawTcpConnection connection in _connections)
			{
				connection.Dispose();
			}
			_connections.Clear();
			Shutdown();
		}
	}
}
