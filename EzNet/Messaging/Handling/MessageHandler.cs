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
		private readonly ConcurrentDictionary<Type, IMessageTypeHandler> _messageHandlers = new ConcurrentDictionary<Type, IMessageTypeHandler>();

		private Task MessageHandlerTask;
		private bool IsDisposed = false;
		public MessageHandler()
		{
			PacketExtension.Init();
			MessageHandlerTask = Task.Run(HandleMessageLoop);
		}

		public void RegisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback)
		{
			callback += ReadPackets;
		}

		public void DeregisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback)
		{
			callback -= ReadPackets;
		}

		public void RegisterMessageType<T>()
			where T : BasePacket, new()
			=> GetOrCreateMessageHandler<T>();
		
		public void AddCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new()
		{
			GetOrCreateMessageHandler<T>().AddCallback(callback);
		}
		
		public void RemoveCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new()
		{
			GetOrCreateMessageHandler<T>().RemoveCallback(callback);
		}

		public int Count<T>() where T : BasePacket, new()
		{
			return GetOrCreateMessageHandler<T>().Count;
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
		
		private MessageTypeHandler<T> GetOrCreateMessageHandler<T>()
			where T : BasePacket, new()
		{
			Type type = typeof(T);
			if (_messageHandlers.TryGetValue(type, out IMessageTypeHandler? handler))
			{
				return ((MessageTypeHandler<T>)handler);
			}
			handler = new MessageTypeHandler<T>();
			_messageHandlers[type] = handler;
			return (MessageTypeHandler<T>)handler;
		}

		private async Task HandleMessageLoop()
		{
			while (IsDisposed == false)
			{
				foreach (IMessageTypeHandler handler in _messageHandlers.Values)
				{
					handler.Update();
				}
				await Task.Delay(1);
			}
		}

		private void ReadPackets(ArraySegment<byte> bytes, Connection source)
		{
			if (bytes.Array == null)
			{
				Log.Warn("Cannot read packets from null buffer");
				return;
			}
			
			using var ms = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count);
			while (ms.Position < ms.Length)
			{
				if (PacketExtension.TryReadType(ms, out Type type) && 
				    _messageHandlers.TryGetValue(type, out var handler))
				{
					handler.ReadPacket(ms, source);
				}
				else
				{
					Log.Warn("No message handler for packet type {0}", type);
				}
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
