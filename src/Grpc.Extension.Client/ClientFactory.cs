using System;
using System.Linq;
using Grpc.Extension.Client.LoadBalance;

namespace Grpc.Extension.Client
{
	public class ClientFactory
	{

		private GrpcLoadBalance GrpcLoadBalance { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		public ClientFactory(GrpcLoadBalance grpcLoadBalance, GrpcClientConfiguration grpcClientConfiguration)
		{
			GrpcLoadBalance = grpcLoadBalance;
			GrpcClientConfiguration = grpcClientConfiguration;
		}



		public T Get<T>(string serviceName)
		{
			var type = GrpcClientConfiguration.ClientTypes.FirstOrDefault(p => p == typeof(T));
			if (type == null)
				throw new InvalidOperationException();
			var channel = GrpcLoadBalance.GetService(serviceName);
			return (T)Activator.CreateInstance(type, channel);
		}
	}
}
