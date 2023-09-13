using System;
using System.IO;

namespace EzNet.Serialization
{
	public interface ISerializer
	{
		public void Serialize(Stream stream, object value);
		public object Deserialize(Stream stream, Type type);
	}
}
