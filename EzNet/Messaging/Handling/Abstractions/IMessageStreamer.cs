using System;

namespace EzNet.Messaging.Handling.Abstractions
{
	public interface IMessageStreamer
	{
		/// <summary>
		/// Register incoming bytes to be decoded
		/// </summary>
		/// <param name="callback"></param>
		public void RegisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback);
		
		/// <summary>
		/// Deregister incoming bytes function
		/// </summary>
		/// <param name="callback"></param>
		public void DeregisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback);
	}
}
