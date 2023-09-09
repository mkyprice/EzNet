using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Requests;

namespace EzNet.Tcp
{
	public class TcpClient : TcpPacketConnection
	{
		protected readonly RequestHandler RequestHandler;
		
		public TcpClient()
		{
			RequestHandler = new RequestHandler(MessageHandler);
		}

		public async Task<TResponse> SendAsync<TResponse, T>(T packet, int timeoutMS = 2000)
			where T : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			var response = await RequestHandler.SendAsync<T, TResponse>(packet, (p) => this.Send(p), timeoutMS);
			return response;
		}

		public void RegisterMessageHandler<TPacket>(Action<TPacket, RawTcpConnection> callback) 
			where TPacket : BasePacket, new()
		{
			MessageHandler.AddCallback<TPacket>((n) =>
			{
				var packet = MessageHandler.Dequeue<TPacket>();
				callback?.Invoke(packet.Message, this);
			});
		}
		
		public void RegisterResponseHandler<TPacket, TResponse>(Func<TPacket, TResponse> callback) 
			where TPacket : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			RequestHandler.RegisterRequest(callback);
		}
	}
}
