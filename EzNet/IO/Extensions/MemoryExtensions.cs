using EzNet.Logging;
using System.IO;

namespace EzNet.IO.Extensions
{
	public static class MemoryExtensions
	{
		public static byte[] ToBytes(this Stream stream)
		{
			byte[] buffer;
			if (stream is MemoryStream ms)
			{
				buffer = ms.ToArray();
			}
			else
			{
				long position = stream.Position;
				buffer = new byte[stream.Length];
				stream.Position = 0;
				int read = stream.Read(buffer, 0, buffer.Length);
				stream.Position = position;
			}
			return buffer;
		}
	}
}
