using System;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Client
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddGrpcClient(this IServiceCollection serviceCollection, Action<GrpcClientConfiguration> configuration)
		{
			var conf = new GrpcClientConfiguration();

			serviceCollection.AddSingleton<ChannelFactory>();
			serviceCollection.AddSingleton(typeof(IGrpcLoadBalancing), conf.GrpcLoadBalancing);
			//serviceCollection.AddSingleton(new );
			configuration?.Invoke(conf);
			serviceCollection.AddSingleton(conf);
			return serviceCollection;
		}
	}
}
