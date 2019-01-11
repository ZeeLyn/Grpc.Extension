using System;
using Grpc.Extension.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddGrpcServer(this IServiceCollection serviceCollection, Action<GrpcServerConfiguration> configure)
		{
			var conf = new GrpcServerConfiguration { ServiceCollection = serviceCollection };
			configure?.Invoke(conf);
			serviceCollection.AddSingleton(conf);
			serviceCollection.AddSingleton<IServerBootstrap, ServerBootstrap>();
			serviceCollection.AddScoped<ServiceProviderMetadataEntry>();
			return serviceCollection;
		}
	}
}
