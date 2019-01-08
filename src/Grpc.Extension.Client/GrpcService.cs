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

		public TResult Client<TClient, TResult>(string serviceName, Func<TClient, TResult> func) where TClient : ClientBase
		{
			var client = ClientFactory.Get<TClient>(serviceName);
			return func(client);
		}
	}
}
