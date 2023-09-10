using EzNet.Logging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Messaging.Requests;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling
{
	public class MessageHandler : IMessageHandler, IDisposable
	{

		public IMessageContainer Container => _container;
		public IMessageStreamer Streamer => _streamer;
		
		private readonly IMessageContainer _container;
		private readonly IMessageStreamer _streamer;
		
		private Task MessageHandlerTask;
		private bool IsDisposed = false;

		private MessageHandler()
		{
		}
		private MessageHandler(IMessageContainer container, IMessageStreamer streamer) : base()
		{
			_container = container;
			_streamer = streamer;
			MessageHandlerTask = Task.Run(HandleMessageLoop);
		}

		public static MessageHandler Build(IMessageContainer? container = null, IMessageStreamer? streamer = null)
		{
			PacketExtension.Init();
			IMessageContainer c = container == null ? new MessageContainer() : container;
			IMessageStreamer s = streamer == null ? new MessageStreamer(c) : streamer;
			return new MessageHandler(c, s);
		}
		public void AddCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new()
		{
			_container.GetOrCreateMessageHandler<T>().AddCallback(callback);
		}
		
		public void RemoveCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new()
		{
			_container.GetOrCreateMessageHandler<T>().RemoveCallback(callback);
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
			AddCallback<RequestPacket>((notification) =>
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

		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest request, Action<BasePacket> sendFunc, int timeoutMS = -1)
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
			AddCallback<ResponsePacket>(ReceiveResponse);
			
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
			RemoveCallback<ResponsePacket>(ReceiveResponse);
			return response;
		}

		private async Task HandleMessageLoop()
		{
			while (IsDisposed == false)
			{
				foreach (IMessageCodec handler in _container.GetCodecs())
				{
					handler.Update();
				}
				await Task.Delay(1);
			}
		}
		public void Dispose()
		{
			if (IsDisposed) return;
			//TODO: Dispose
			IsDisposed = true;
		}
	}
}
