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
			conf.Services.Add(Health.V1.Health.BindService(new HealthCheckService.HealthCheckService()));
			return serviceCollection;
		}
	}
}
