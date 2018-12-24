using System;
using Grpc.Extension.Client.LoadBalancer;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Client
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddGrpcClient(this IServiceCollection serviceCollection, Action<GrpcClientConfiguration> configuration)
		{
			var conf = new GrpcClientConfiguration();
			serviceCollection.AddSingleton<ChannelFactory>();
			serviceCollection.AddSingleton<ClientFactory>();
			serviceCollection.AddSingleton(typeof(ILoadBalancer), conf.GrpcLoadBalance);
			configuration?.Invoke(conf);
			serviceCollection.AddSingleton(conf);
			return serviceCollection;
		}
	}
}
