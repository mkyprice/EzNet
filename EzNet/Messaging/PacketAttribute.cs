using EzNet.Logging;

namespace EzNet.Messaging
{
	/// <summary>
	/// Describes a packet
	/// This should match the packet ID on clients/servers and should always be unique
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class PacketAttribute : Attribute
	{
		/// <summary>
		/// Unique ID to type of packet
		/// </summary>
		public string Id;
		
		private PacketAttribute() { }
		
		public PacketAttribute(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				Log.Warn("Using {0} with no name invalidates purpose", nameof(PacketAttribute));
			}
			Id = id;
		}
	}
}
