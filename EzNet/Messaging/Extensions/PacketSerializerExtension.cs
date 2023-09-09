﻿using EzNet.IO;
using EzNet.IO.Extensions;
using EzNet.Logging;
using System.Reflection;

namespace EzNet.Messaging
{
	public static class PacketSerializerExtension
	{
		public static IEnumerable<Type> AssemblyPacketTypes => _keyToPacket.Values;
		private static readonly Dictionary<string, Type> _keyToPacket = new Dictionary<string, Type>();
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

		public static byte[] Serialize<T>(T packet) where T : BasePacket
		{
			Type type = packet.GetType();
			string key = GetKey(type);
			using Stream ms = new MemoryStream();
			ms.Write(key);
			packet.Write(ms);
			byte[] bytes = ms.ToBytes(0, (int)ms.Length);
			return bytes;
		}

		public static Type ReadPacketType(Stream stream)
		{
			string key = stream.ReadString();
			return _keyToPacket[key];
		}

		public static BasePacket Deserialize(Stream stream)
		{
			string key = stream.ReadString();
			if (_keyToPacket.TryGetValue(key, out Type type))
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

		private static string GetKey(Type type) => type.Name;
	}
}
