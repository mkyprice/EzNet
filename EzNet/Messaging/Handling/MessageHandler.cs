using EzNet.Logging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Messaging.Handling.Utils;
using EzNet.Messaging.Requests;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
			AddCallback<RequestPacket>(responseHandler.OnRequest);
			_responseHandlers.Add(responseHandler);
		}

		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest request, Func<BasePacket, bool> sendFunc, int timeoutMs = 2000)
			where TResponse : BasePacket, new()
			where TRequest : BasePacket, new()
		{
			// Build request packet
			RequestPacket requestPacket = new RequestPacket(request);
			
			// Receiving
			TaskCompletionSource<TResponse> taskCompletionSource = new TaskCompletionSource<TResponse>();
			void ReceiveResponse(ResponsePacket responsePacket, Connection source)
			{
				// Ensure matching IDs
				if (responsePacket.RequestId == requestPacket.RequestId)
				{
					if (responsePacket.Response is TResponse r)
					{
						taskCompletionSource.SetResult(r);
					}
					else
					{
						Log.Error("Received incorrect response type {0}", responsePacket.Response);
						taskCompletionSource.SetResult(new TResponse()
						{
							Error = PACKET_ERROR.BadResponse
						});
					}
				}
			}
			// Register response
			AddCallback<ResponsePacket>(ReceiveResponse);
			
			TResponse response;
			// Send out request
			if (sendFunc(requestPacket))
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
			RemoveCallback<ResponsePacket>(ReceiveResponse);
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
					foreach (IMessageCodec codec in _container.GetCodecs())
					{
						codec.Update();
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
