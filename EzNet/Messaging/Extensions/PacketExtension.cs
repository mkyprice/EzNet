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
		public static readonly string REQUEST_ID = "REQUEST_ID";
		
		private static readonly BiDiDictionary<string, Type> _packetKeys = new BiDiDictionary<string, Type>();
		private static bool _isInitialized = false;
		public static void Init()
		{
			if (_isInitialized) return;
			_isInitialized = true;

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					RegisterType(type);
				}
			}
		}

		public static void RegisterType(Type type)
		{
			Type packet_base_type = typeof(BasePacket);
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
					string key;
					var attrib = type.GetCustomAttribute<PacketAttribute>();
					if (attrib != null && string.IsNullOrEmpty(attrib.Id) == false)
					{
						key = attrib.Id;
					}
					else
					{
						key = type.Name;
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

		public static void Serialize<T>(Stream stream, T packet) 
			where T : BasePacket
		{
			long start = stream.Position;
			stream.Position += sizeof(uint);
			
			// Write packet
			Type type = packet?.GetType() ?? typeof(T);
			WriteType(stream, type);
			packet?.Write(stream);

			// Calculate and write packet size
			long end = stream.Position;
			uint size = (uint)(end - start);
			stream.Position = start;
			stream.Write(size);
			stream.Position = end;
		}

		public static BasePacket? Deserialize(Stream stream)
		{
			long start = stream.Position;
			uint length = stream.ReadUInt();
			if (stream.Length >= length && TryReadType(stream, out Type type))
			{
				BasePacket packet = (BasePacket)type.NewInstance();
				if (packet != null)
				{
					packet.Read(stream);
					return packet;
				}
			}
			stream.Position = start + length;
			return null;
		}

		public static bool TryReadType(Stream stream, out Type type) 
			=> _packetKeys.TryGetValue(stream.ReadString(), out type);
		
		private static void WriteType(Stream stream, in Type type) 
			=> stream.Write(GetKey(type));
		
		private static string GetKey(in Type type) 
			=> _packetKeys[type];
	}
}
