using System;
using System.IO;
using System.Xml.Serialization;

namespace EzNet.Serialization
{
	public class XmlByteSerializer : ISerializer
	{
		public void Serialize(Stream stream, object value)
		{
			if (value != null)
			{
				Type type = value.GetType();
				XmlSerializer serializer = new XmlSerializer(type);
				serializer.Serialize(stream, value);
			}
		}
		
		public object Deserialize(Stream stream, Type type)
		{
			XmlSerializer serializer = new XmlSerializer(type);
			return serializer.Deserialize(stream);
		}
	}
}
