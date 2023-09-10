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
					((PacketConnection)notification.Source).Send(responsePacket);
				}
			});
		}

		internal async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, Action<RequestPacket> sendFunc, int timeoutMS = 2000)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			// Build request packet
			RequestPacket requestPacket = new RequestPacket(request);
			
			// Receiving
			TaskCompletionSource<TResponse> taskCompletionSource = new TaskCompletionSource<TResponse>();
			void ReceiveResponse(MessageNotification<ResponsePacket> response)
			{
				// Ensure matching IDs
				if (response.Message.RequestId == requestPacket.RequestId)
				{
					if (response.Message.Packet is TResponse r)
					{
						taskCompletionSource.SetResult(r);
					}
					else
					{
						Log.Error("Received incorrect response type {0}", response.Message.Packet);
						taskCompletionSource.SetCanceled();
					}
				}
			}
			// Register response
			MessageHandler.AddCallback<ResponsePacket>(ReceiveResponse);
			
			// Send out request
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
			
			// Await the result
			TResponse result = await taskCompletionSource.Task;
			
			// Cleanup
			MessageHandler.RemoveCallback<ResponsePacket>(ReceiveResponse);
			return result;
		}
	}
}
