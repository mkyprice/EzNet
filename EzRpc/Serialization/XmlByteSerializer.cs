using EzRpc.Serialization.Extensions;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EzRpc.Serialization
{
	public class XmlByteSerializer : ISerializer
	{
		private static XmlWriterSettings Settings = new XmlWriterSettings()
		{
			Indent = false
		};
		public void Serialize(Stream stream, object value)
		{
			if (value != null)
			{
				Type type = value.GetType();
				if (type.IsPrimitive)
				{
					stream.WritePrimitive(value);
				}
				else
				{
					XmlSerializer serializer = new XmlSerializer(type);
					serializer.Serialize(stream, value);
				}
			}
		}
		
		public object Deserialize(Stream stream, Type type)
		{
			if (type.IsPrimitive)
			{
				return stream.ReadPrimitive(type);
			}
			XmlSerializer serializer = new XmlSerializer(type);
			return serializer.Deserialize(stream);
		}
	}
}
