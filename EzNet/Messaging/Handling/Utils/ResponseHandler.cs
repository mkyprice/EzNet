using EzNet.Logging;
using EzNet.Messaging.Extensions;
using System;
using System.Collections.Generic;

namespace EzNet.Messaging.Handling.Utils
{
	internal interface IResponseHandler
	{
		public void OnRequest(BasePacket requestPacket, Connection source);
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
		
		public void OnRequest(BasePacket requestPacket, Connection source)
		{
			// Respond to type request
			if (requestPacket is TRequest request)
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
					response = new TResponse()
					{
						Error = PACKET_ERROR.BadResponse
					};
				}
				response.AddMeta(PacketExtension.REQUEST_ID, requestPacket.Meta[PacketExtension.REQUEST_ID]);
				if (source.Send(response) == false)
				{
					Log.Warn("Failed to send response {0}", response);
				}
			}
		}
		
		public override bool Equals(object? obj)
		{
			return obj is ResponseHandler<TRequest, TResponse> rh && rh._function == _function;
		}
	}
}
