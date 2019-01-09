using System;
using Grpc.Core;

namespace Grpc.Extension.Client
{
	public class GrpcService
	{
		private ClientFactory ClientFactory { get; }
		public GrpcService(ClientFactory clientFactory)
		{
			ClientFactory = clientFactory;
		}


	}
}
