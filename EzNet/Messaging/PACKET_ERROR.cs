namespace EzNet.Messaging
{
	public enum PACKET_ERROR : byte
	{
		NoError = 0,
		BadResponse = 1,
		Timeout = 2,
		SendFailed = 3
	}
}
