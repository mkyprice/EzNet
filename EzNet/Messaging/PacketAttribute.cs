using EzNet.Logging;

namespace EzNet.Messaging
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PacketAttribute : Attribute
	{
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
