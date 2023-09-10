using System;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling.Abstractions
{
	public interface IMessageHandler : IDisposable
	{
		public void RegisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback);
		public void DeregisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback);
		public void RegisterMessageType<T>() where T : BasePacket, new();
		public void AddCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new();
		public void RemoveCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new();
		public int Count<T>() where T : BasePacket, new();
		
		public void RegisterRequest<TRequest, TResponse>(Func<TRequest, TResponse> requestFunc)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new();
		
		public Task<TResponse> SendAsync<TResponse, TRequest>(TRequest request, Action<BasePacket> sendFunc, int timeoutMS = -1)
			where TResponse : BasePacket, new()
			where TRequest : BasePacket, new();
	}
}
