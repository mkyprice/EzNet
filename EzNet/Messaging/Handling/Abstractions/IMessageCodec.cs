using System.IO;

namespace EzNet.Messaging.Handling.Abstractions
{
	public interface IMessageCodec
	{
		/// <summary>
		/// Read a single packet from a stream
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="source"></param>
		public void Enqueue(BasePacket packet, Connection source);

		/// <summary>
		/// Create instance of codec type packet
		/// </summary>
		/// <returns></returns>
		public BasePacket CreatePacket();
		
		/// <summary>
		/// Performs dequeue and events
		/// </summary>
		public void Update();
	}
}
