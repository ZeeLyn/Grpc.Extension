using System;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddGrpcServer(this IServiceCollection serviceCollection, Action<GrpcServerConfiguration> configure)
		{
			var conf = new GrpcServerConfiguration();
			configure?.Invoke(conf);
			serviceCollection.AddSingleton(conf);
			serviceCollection.AddSingleton<GrpcServer>();
			serviceCollection.AddSingleton<HealthCheckService.HealthCheckService>();
			serviceCollection.AddScoped<ServiceProviderMetadataEntry>();
			return serviceCollection;
		}
	}
}
