using EzNet.Logging;
using EzNet.Messaging.Requests;
using System;

namespace EzNet.Messaging.Handling.Utils
{
	internal interface IResponseHandler
	{
		public void OnRequest(RequestPacket requestPacket, Connection source);
	}
	
	internal class ResponseHandler<TRequest, TResponse> : IResponseHandler
		where TRequest : BasePacket, new()
		where TResponse : BasePacket, new()
	{
		private readonly Func<TRequest, TResponse> _function;
		
		public ResponseHandler(Func<TRequest, TResponse> function)
		{
			_function = function;
		}
		
		public void OnRequest(RequestPacket requestPacket, Connection source)
		{
			// Respond to type request
			if (requestPacket.Packet is TRequest request)
			{
				BasePacket response;
				try
				{
					response = _function.Invoke(request);
				}
				catch (Exception e)
				{
					Log.Error("Response function: <{0}, {1}> encountered error: {2}\nTrace: - {3}", 
						typeof(TResponse), typeof(TRequest), e.Message, e.StackTrace);
					response = new ErrorPacket(PACKET_ERROR.BadResponse);
				}
				ResponsePacket responsePacket = new ResponsePacket(response, requestPacket.RequestId);
				if (source.Send(responsePacket) == false)
				{
					Log.Warn("Failed to send response {0}", response);
				}
			}
		}
	}
}
