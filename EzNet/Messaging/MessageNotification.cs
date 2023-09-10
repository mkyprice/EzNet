namespace EzNet.Messaging
{
	public interface IMessageNotification
	{
		
	}
	public readonly struct MessageNotification<T> : IMessageNotification
		where T : BasePacket, new()
	{
		public readonly T Message;
		public readonly PacketConnection Source;

		public MessageNotification(T message, PacketConnection source)
		{
			Message = message;
			Source = source;
		}
	}
}
