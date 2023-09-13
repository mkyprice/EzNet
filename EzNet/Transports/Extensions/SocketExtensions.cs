using System.Net;
using System.Net.Sockets;

namespace EzNet.Transports.Extensions
{
	public static class SocketExtensions
	{
		/// <summary>
		/// Create endpoint given port
		/// </summary>
		/// <param name="port"></param>
		/// <param name="family">[WARNING]InterNetwork is the only type currently implemented</param>
		/// <returns></returns>
		public static EndPoint GetEndPoint(int port, AddressFamily family = AddressFamily.InterNetwork)
		{
			string host_name = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host_name);
			IPAddress address = null;
			for (int i = 0; i < ipHost.AddressList.Length; i++)
			{
				if (ipHost.AddressList[i].AddressFamily == family)
				{
					address = ipHost.AddressList[i];
					break;
				}
			}
			IPEndPoint endPoint = new IPEndPoint(address, port);
			return endPoint;
		}
	}
}
