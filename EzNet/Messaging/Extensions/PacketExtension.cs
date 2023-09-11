using EzNet.IO;
using EzNet.IO.Extensions;
using EzNet.Logging;
using EzNet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EzNet.Messaging.Extensions
{
	internal static class PacketExtension
	{
		private static readonly BiDiDictionary<int, Type> _packetKeys = new BiDiDictionary<int, Type>();
		private static bool _isInitialized = false;
		public static void Init()
		{
			if (_isInitialized) return;
			_isInitialized = true;

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
							int key;
							var attrib = type.GetCustomAttribute<PacketAttribute>();
							if (attrib != null && string.IsNullOrEmpty(attrib.Id) == false)
							{
								key = attrib.Id.GetHashCode();
							}
							else
							{
								key = type.Name.GetHashCode();
							}
							_packetKeys[key] = type;
							Log.Debug("Cached packet type {0}", type);
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
			Type type = packet?.GetType() ?? typeof(T);
			WriteType(stream, type);
			packet?.Write(stream);
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
			if (seg.Array == null)
			{
				Log.Warn("Tried to deserialize a null array");
				yield break;
			} 
			byte[] bytes = seg.Array;
			using (var ms = new MemoryStream(bytes))
			{
				while (ms.Position + sizeof(int) < seg.Count)
				{
					BasePacket? packet = Deserialize(ms);
					if (packet != null)
					{
						yield return packet;
					}
				}
			}
		}


		public static bool TryReadType(Stream? stream, out Type type) => _packetKeys.TryGetValue(stream.ReadInt(), out type);
		private static void WriteType(Stream? stream, in Type type) => stream.Write(GetKey(type));
		private static int GetKey(in Type type) => _packetKeys[type];
	}
}
