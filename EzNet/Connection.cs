using EzNet.Messaging;

namespace EzNet
{
	public abstract class Connection : IDisposable
	{
		public readonly MessageHandler MessageHandler = new MessageHandler();
		public abstract bool IsConnected { get; }
		protected bool IsDisposed { get; private set; } = false;
		
		private Task PacketReaderTask;

		public Connection()
		{
			PacketReaderTask = Task.Run(ReadMessageQueue);
		}

		public void RegisterMessageHandler<TPacket>(Action<TPacket, Connection> callback) where TPacket : BasePacket, new()
		{
			MessageHandler.RegisterMessageCallback<TPacket>((packet) =>
			{
				callback?.Invoke(packet, this);
			});
		}

		protected abstract bool TryDequeuePacket(out byte[] packet);

		private async Task ReadMessageQueue()
		{
			while (IsDisposed == false)
			{
				if (TryDequeuePacket(out var packet))
				{
					MessageHandler.ReadPackets(packet, packet.Length);
				}
				else
				{
					await Task.Delay(1);
				}
			}
		}
		
		public void Dispose()
		{
			IsDisposed = true;
		}
	}
}
