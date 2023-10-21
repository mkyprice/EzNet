using EzNet.Logging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Messaging.Handling.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling
{
	public class MessageHandler : IMessageHandler, IDisposable
	{
		private readonly List<IResponseHandler> _responseHandlers = new List<IResponseHandler>();
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
			MessageHandlerTask = Task.Factory.StartNew(HandleMessageLoop, TaskCreationOptions.LongRunning);
		}

		public static MessageHandler Build(IMessageContainer? container = null, IMessageStreamer? streamer = null)
		{
			PacketExtension.Init();
			IMessageContainer c = container ?? new MessageContainer();
			IMessageStreamer s = streamer ?? new MessageStreamer(c);
			return new MessageHandler(c, s);
		}
		public void AddCallback<T>(Action<T, Connection> callback) where T : BasePacket, new()
		{
			_container.GetOrCreateMessageHandler<T>().AddCallback(callback);
		}
		
		public void RemoveCallback<T>(Action<T, Connection> callback) where T : BasePacket, new()
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
			ResponseHandler<TRequest, TResponse> responseHandler = new ResponseHandler<TRequest, TResponse>(requestFunc);
			AddCallback<TRequest>(responseHandler.OnRequest);
			_responseHandlers.Add(responseHandler);
		}

		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest request, int timeoutMs = 2000)
			where TResponse : BasePacket, new()
			where TRequest : BasePacket, new()
		{
			// Build request packet
			string requestId = Guid.NewGuid().ToString();
			request.AddMeta(PacketExtension.REQUEST_ID, requestId);
			
			// Receiving
			TaskCompletionSource<TResponse> taskCompletionSource = new TaskCompletionSource<TResponse>();
			void ReceiveResponse(TResponse responsePacket, Connection source)
			{
				// Ensure matching IDs
				if (responsePacket.Meta?.TryGetValue(PacketExtension.REQUEST_ID, out string id) == true && id == requestId)
				{
					taskCompletionSource.SetResult(responsePacket);
				}
			}
			// Register response
			AddCallback<TResponse>(ReceiveResponse);
			
			TResponse response;
			// Send out request
			if (_streamer.WriteMessage(request))
			{
				response = await AwaitTaskOrTimeout(taskCompletionSource, timeoutMs);
				if (response == null)
				{
					Log.Warn("Send timed out {0}", request);
					response = new TResponse()
					{
						Error = PACKET_ERROR.Timeout
					};
				}
			}
			else
			{
				Log.Error("Failed to send request {0}", request);
				response = new TResponse()
				{
					Error = PACKET_ERROR.SendFailed
				};
			}
			
			
			// Cleanup
			RemoveCallback<TResponse>(ReceiveResponse);
			return response;
		}

		/// <summary>
		/// Waits for the task or times out if it takes too long
		/// </summary>
		/// <param name="taskSource"></param>
		/// <param name="timeoutMs">timeout in milliseconds. -1 for no timeout</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private async Task<T> AwaitTaskOrTimeout<T>(TaskCompletionSource<T> taskSource, int timeoutMs)
		{
			T result;
			if (timeoutMs >= 0)
			{
				using (CancellationTokenSource cancellation = new CancellationTokenSource(timeoutMs))
				{
					Task task = await Task.WhenAny(taskSource.Task, Task.Delay(timeoutMs, cancellation.Token));
					if (task == taskSource.Task)
					{
						cancellation.Cancel();
						result = await taskSource.Task;
					}
					else
					{
						result = default(T);
						Log.Warn("Response {0} timed out", typeof(T));
					}
				}
			}
			else
			{
				result = await taskSource.Task;
			}
			return result;
		}

		private async Task HandleMessageLoop()
		{
			try
			{
				while (IsDisposed == false)
				{
					foreach (IMessageQueue codec in _container.GetMessageQueues())
					{
						codec.DequeueMessages();
					}
					await Task.Delay(1);
				}
			}
			catch (Exception e)
			{
				Log.Fatal("Encountered error: {0}\nTrace - {1}", e.Message, e.StackTrace);
			}
		}
		public void Dispose()
		{
			if (IsDisposed) return;
			_responseHandlers.Clear();
			//TODO: Dispose
			IsDisposed = true;
		}
	}
}
