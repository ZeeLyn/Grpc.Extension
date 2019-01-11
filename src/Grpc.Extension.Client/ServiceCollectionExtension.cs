using System;
using Grpc.Extension.Client.CircuitBreaker;
using Grpc.Extension.Client.LoadBalancer;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Client
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddGrpcClient(this IServiceCollection serviceCollection, Action<GrpcClientConfiguration> configuration)
		{
			var conf = new GrpcClientConfiguration { ServiceCollection = serviceCollection };
			serviceCollection.AddSingleton<IChannelFactory, ChannelFactory>();
			serviceCollection.AddSingleton<IClientFactory, ClientFactory>();
			serviceCollection.AddSingleton<CircuitBreakerServiceBuilder>();
			serviceCollection.AddSingleton<CircuitBreakerPolicy>();
			configuration?.Invoke(conf);
			serviceCollection.AddSingleton(conf);
			if (conf.GrpcLoadBalance != null)
				serviceCollection.AddSingleton(typeof(ILoadBalancer), conf.GrpcLoadBalance);
			return serviceCollection;
		}
	}
}
