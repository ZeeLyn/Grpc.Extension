using System;
using System.Linq;
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


		public T Get<T>(string serviceName)
		{
			var type = GrpcClientConfiguration.ClientTypes.FirstOrDefault(p => p == typeof(T));
			if (type == null)
				throw new InvalidOperationException($"Not found client {typeof(T)}.");
			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			return (T)Activator.CreateInstance(type, channel);
		}
	}
}
