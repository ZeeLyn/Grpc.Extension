using System;
using System.Linq;


namespace Grpc.Extension.Client
{
	public class ClientFactory
	{

		private LoadBalancer.ILoadBalancer GrpcLoadBalance { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		public ClientFactory(LoadBalancer.ILoadBalancer grpcLoadBalance, GrpcClientConfiguration grpcClientConfiguration)
		{
			GrpcLoadBalance = grpcLoadBalance;
			GrpcClientConfiguration = grpcClientConfiguration;
		}



		public T Get<T>(string serviceName)
		{
			var type = GrpcClientConfiguration.ClientTypes.FirstOrDefault(p => p == typeof(T));
			if (type == null)
				throw new InvalidOperationException();
			var channel = GrpcLoadBalance.GetChannel(serviceName);
			return (T)Activator.CreateInstance(type, channel);
		}
	}
}
