using System.IO;

namespace EzNet.Messaging.Handling.Abstractions
{
	public interface IMessageCodec
	{
		/// <summary>
		/// Read a single packet from a stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="source"></param>
		public void ReadPacket(Stream stream, Connection source);
		
		/// <summary>
		/// Performs dequeue and events
		/// </summary>
		public void Update();
	}
}
