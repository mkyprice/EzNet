using System;

namespace EzNet.Messaging.Handling.Abstractions
{
	public interface IMessageStreamer
	{
		public bool WriteMessage(BasePacket packet);
		
		/// <summary>
		/// Register incoming bytes to be decoded
		/// </summary>
		/// <param name="callback"></param>
		public void RegisterMessageReader(ref Action<ArraySegment<byte>, Connection> callback);
		
		/// <summary>
		/// Deregister incoming bytes function
		/// </summary>
		/// <param name="callback"></param>
		public void DeregisterMessageReader(ref Action<ArraySegment<byte>, Connection> callback);
		
		public void RegisterByteSender(Func<byte[], bool> send);
		public void DeregisterByteSender(Func<byte[], bool> send);
	}
}
