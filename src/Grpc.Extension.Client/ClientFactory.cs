using System;
using System.Linq;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalancer;


namespace Grpc.Extension.Client
{
	public class ClientFactory
	{

		private ILoadBalancer GrpcLoadBalance { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		public ClientFactory(ILoadBalancer grpcLoadBalance, GrpcClientConfiguration grpcClientConfiguration)
		{
			GrpcLoadBalance = grpcLoadBalance;
			GrpcClientConfiguration = grpcClientConfiguration;
		}


		public TGrpcClient Get<TGrpcClient>(string serviceName) where TGrpcClient : ClientBase
		{
			var type = GrpcClientConfiguration.ClientTypes.FirstOrDefault(p => p == typeof(TGrpcClient));
			if (type == null)
				throw new InvalidOperationException($"Not found client {typeof(TGrpcClient)}.");
			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			return (TGrpcClient)Activator.CreateInstance(type, channel);
		}
	}
}
