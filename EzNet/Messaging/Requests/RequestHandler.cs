using EzNet.Logging;
using EzNet.Tcp;

namespace EzNet.Messaging.Requests
{
	public class RequestHandler
	{
		public readonly MessageHandler MessageHandler;

		public RequestHandler(MessageHandler handler)
		{
			MessageHandler = handler;
			MessageHandler.GetOrCreateMessageHandler<ResponsePacket>();
		}
		
		public void RegisterRequest<TRequest, TResponse>(Func<TRequest, TResponse> requestFunc)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			MessageHandler.AddCallback<RequestPacket>((notification) =>
			{
				if (notification.Message.Packet is TRequest request)
				{
					TResponse response = requestFunc(request);
					ResponsePacket responsePacket = new ResponsePacket(response, notification.Message.RequestId);
					((TcpPacketConnection)notification.Args).Send(responsePacket);
				}
			});
		}

		internal async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, Action<RequestPacket> sendFunc, int timeoutMS = 2000)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			RequestPacket requestPacket = new RequestPacket(request);
			// Receiving
			TaskCompletionSource<TResponse> taskCompletionSource = new TaskCompletionSource<TResponse>();
			
			void ReceiveResponse(MessageNotification<ResponsePacket> response)
			{
				if (response.Message.RequestId == requestPacket.RequestId)
				{
					taskCompletionSource.SetResult((TResponse)response.Message.Packet);
				}
			}
			
			MessageHandler.AddCallback<ResponsePacket>(ReceiveResponse);
			sendFunc(requestPacket);
			
			// Timeout loop
			DateTime timeout = DateTime.Now.AddMilliseconds(timeoutMS);
			while (taskCompletionSource.Task.IsCompleted == false)
			{
				await Task.Yield();
				if (DateTime.Now >= timeout)
				{
					taskCompletionSource.SetResult(default);
					Log.Warn("Failed to get response");
					break;
				}
			}
			
			var result = await taskCompletionSource.Task;
			
			MessageHandler.RemoveCallback<ResponsePacket>(ReceiveResponse);
			return result;
		}
	}
}
