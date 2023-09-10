using EzNet.IO;
using EzNet.IO.Extensions;
using EzNet.Logging;
using System.Reflection;

namespace EzNet.Messaging
{
	public static class PacketSerializerExtension
	{
		public static IEnumerable<Type> AssemblyPacketTypes => _keyToPacket.Values;
		private static readonly Dictionary<int, Type> _keyToPacket = new Dictionary<int, Type>();
		private static bool IsInitialized = false;
		public static void Init()
		{
			if (IsInitialized) return;
			IsInitialized = true;

			Type packet_base_type = typeof(BasePacket);
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (packet_base_type.IsAssignableFrom(type))
					{
						bool can_create = false;
						foreach (ConstructorInfo constructor in type.GetConstructors())
						{
							if (constructor.GetParameters().Length <= 0)
							{
								can_create = true;
								break;
							}
						}
						if (can_create)
						{
							_keyToPacket[GetKey(type)] = type;
							Log.Info("Cached packet type {0}", type);
						}
						else if (type.IsAbstract == false)
						{
							Log.Warn("Unable to create PacketType {0}. Please ensure it has a default constructor", type);
						}
					}
				}
			}
		}

		public static void Serialize<T>(Stream stream, T packet) 
			where T : BasePacket
		{
			Type type = packet.GetType();
			WriteType(stream, type);
			packet.Write(stream);
		}

		public static BasePacket Deserialize(Stream stream)
		{
			if (TryReadType(stream, out Type type))
			{
				BasePacket packet = (BasePacket)Activator.CreateInstance(type);
				if (packet != null)
				{
					packet.Read(stream);
					return packet;
				}
			}
			return null;
		}

		public static IEnumerable<BasePacket> Deserialize(ArraySegment<byte> seg)
		{
			byte[] bytes = seg.ToArray();
			using var ms = new MemoryStream(bytes);
			while (ms.Position + sizeof(int) < ms.Length)
			{
				BasePacket? packet = Deserialize(ms);
				if (packet != null)
				{
					yield return packet;
				}
			}
		}


		public static bool TryReadType(Stream stream, out Type type) => _keyToPacket.TryGetValue(stream.ReadInt(), out type);
		private static void WriteType(Stream stream, in Type type) => stream.Write(GetKey(type));
		private static int GetKey(in Type type) => type.Name.GetHashCode();
	}
}
