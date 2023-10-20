using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EzRpc.Serialization
{
	public interface ISerializer
	{
		/// <summary>
		/// Serialize object to stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="value"></param>
		public void Serialize(Stream stream, object value);
		
		/// <summary>
		/// Deserialize object of type from stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public object Deserialize(Stream stream, Type type);
	}
}
