using EzNet.Logging;
using EzNet.Messaging.Handling;
using System;
using System.Threading;
using System.Threading.Tasks;

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
			if (requestFunc == null)
			{
				Log.Warn("Response function<{0}, {1}> was null", typeof(TRequest).Name, typeof(TResponse).Name);
				return;
			}
			MessageHandler.AddCallback<RequestPacket>((notification) =>
			{
				// Respond to type request
				if (notification.Message.Packet is TRequest request)
				{
					TResponse response = requestFunc(request);
					ResponsePacket responsePacket = new ResponsePacket(response, notification.Message.RequestId);
					notification.Source.Send(responsePacket);
				}
			});
		}

		internal async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest request, Action<RequestPacket> sendFunc, int timeoutMS = -1)
			where TResponse : BasePacket, new()
			where TRequest : BasePacket, new()
		{
			// Build request packet
			RequestPacket requestPacket = new RequestPacket(request);
			
			// Receiving
			TaskCompletionSource<TResponse> taskCompletionSource = new TaskCompletionSource<TResponse>();
			void ReceiveResponse(MessageNotification<ResponsePacket> responsePacket)
			{
				// Ensure matching IDs
				if (responsePacket.Message.RequestId == requestPacket.RequestId)
				{
					if (responsePacket.Message.Packet is TResponse r)
					{
						taskCompletionSource.SetResult(r);
					}
					else
					{
						Log.Error("Received incorrect response type {0}", responsePacket.Message.Packet);
						taskCompletionSource.SetCanceled();
					}
				}
			}
			// Register response
			MessageHandler.AddCallback<ResponsePacket>(ReceiveResponse);
			
			// Send out request
			sendFunc(requestPacket);
			
			// Await the result
			TResponse response;
			if (timeoutMS >= 0)
			{
				using (var cancellation = new CancellationTokenSource(timeoutMS))
				{
					Task task = await Task.WhenAny(taskCompletionSource.Task, Task.Delay(timeoutMS, cancellation.Token));
					if (task == taskCompletionSource.Task)
					{
						cancellation.Cancel();
						response = await taskCompletionSource.Task;
					}
					else
					{
						response = default(TResponse);
						Log.Warn("Request {0} timed out", request);
					}
				}
			}
			else
			{
				response = await taskCompletionSource.Task;
			}
			
			// Cleanup
			MessageHandler.RemoveCallback<ResponsePacket>(ReceiveResponse);
			return response;
		}
	}
}
