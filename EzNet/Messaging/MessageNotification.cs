namespace EzNet.Messaging
{
	public interface IMessageNotification
	{
		
	}
	public readonly struct MessageNotification<T> : IMessageNotification
		where T : BasePacket, new()
	{
		public readonly T Message;
		public readonly object Args;

		public MessageNotification(T message, object args)
		{
			Message = message;
			Args = args;
		}
	}
}
