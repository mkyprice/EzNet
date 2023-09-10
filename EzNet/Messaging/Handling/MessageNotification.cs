namespace EzNet.Messaging.Handling
{
	public interface IMessageNotification
	{
		
	}
	public readonly struct MessageNotification<T> : IMessageNotification
		where T : BasePacket, new()
	{
		public readonly T Message;
		public readonly Connection Source;

		public MessageNotification(T message, Connection source)
		{
			Message = message;
			Source = source;
		}
	}
}
