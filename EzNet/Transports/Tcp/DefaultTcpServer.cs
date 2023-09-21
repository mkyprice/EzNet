using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Transports.Tcp
{
	internal class DefaultTcpServer : ITransportServer
	{
		public Action<ITransportConnection> OnNewConnection { get; set; }
		public Action<ITransportConnection> OnEndConnection { get; set; }

		private readonly Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly List<ITransportConnection> _connections = new List<ITransportConnection>();

		
		public void Listen(int port)
		{
			EndPoint endPoint = SocketExtensions.GetEndPoint(port, AddressFamily.InterNetwork);
			Listen(endPoint);
		}
		
		public void Listen(EndPoint endPoint)
		{
			Listener.Bind(endPoint);
			Listener.Listen(100);

			Listener.BeginAccept(OnConnection, Listener);
		}

		public bool Send(byte[] bytes)
		{
			foreach (ITransportConnection connection in _connections)
			{
				connection.Send(bytes);
			}
			return true;
		}
		
		public void Shutdown()
		{
			if (Listener.Connected == false)
			{
				return;
			}
			Listener.Shutdown(SocketShutdown.Both);
		}
		
		public void Dispose()
		{
			foreach (var connection in _connections)
			{
				connection.Dispose();
			}
			_connections.Clear();
			Shutdown();
		}

		#region Private Functions

		private void OnConnection(IAsyncResult result)
		{
			Socket socket = ((Socket)result.AsyncState).EndAccept(result);
			ITransportConnection connection = new DefaultTcpConnection(socket);
			connection.OnDisconnect += OnDisconnect;
			_connections.Add(connection);

			OnNewConnection?.Invoke(connection);
			
			Listener.BeginAccept(OnConnection, Listener);
		}
		
		private void OnDisconnect(ITransportConnection obj)
		{
			obj.OnDisconnect -= OnDisconnect;
			try
			{
				_connections.Remove(obj);
			}
			catch (Exception e)
			{
				Log.Error(e.Message);
			}
			OnEndConnection?.Invoke(obj);
		}

		#endregion
	}
}
