using EzNet.Logging;
using EzNet.Messaging;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	public class TcpServer : IDisposable
	{
		private readonly List<TcpPeer> _connections = new List<TcpPeer>();
		private readonly Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		public void Listen(EndPoint endPoint)
		{
			Listener.Bind(endPoint);
			Listener.Listen(100);

			Listener.BeginAccept(OnConnection, Listener);
		}

		public void Broadcast<T>(T packet)
			where T : BasePacket
		{
			byte[] bytes = PacketSerializerExtension.Serialize(packet);
			Console.WriteLine("Sending {0} bytes", bytes.Length);
			foreach (TcpPeer connection in _connections)
			{
				connection.Send(bytes);
			}
		}

		private void OnConnection(IAsyncResult result)
		{
			Socket socket = ((Socket)result.AsyncState).EndAccept(result);
			TcpPeer connection = new TcpPeer(socket, this);
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
			foreach (TcpRawConnection connection in _connections)
			{
				connection.Dispose();
			}
			_connections.Clear();
			Shutdown();
		}
	}
}
