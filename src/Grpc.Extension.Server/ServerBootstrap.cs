using System;
using Grpc.Core.Interceptors;
using Grpc.Extension.Core;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Server
{
	internal class ServerBootstrap : IServerBootstrap
	{

		private ILogger Logger { get; }

		public ServerBootstrap(ILogger<ServerBootstrap> logger)
		{
			Logger = logger;
		}
		public void Start(IApplicationBuilder app)
		{
			try
			{
				var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
				var configure = app.ApplicationServices.GetService<GrpcServerConfiguration>();
				if (configure.ServerPort == null)
					throw new ArgumentNullException(nameof(configure.ServerPort));
				var discovery = app.ApplicationServices.GetService<IServiceDiscovery>();
				Logger.LogInformation("---------------> Grpc server is starting...");

				var server = new Grpc.Core.Server
				{
					Ports = { configure.ServerPort },
					Services = { MagicOnionEngine.BuildServerServiceDefinition(true).ServerServiceDefinition.Intercept(new DependencyInjectionInterceptor(app.ApplicationServices)) }
				};


				//Register health check service
				server.Services.Add(Health.V1.Health.BindService(app.ApplicationServices.GetService<HealthCheckService.HealthCheckService>()));

				//Stop service
				applicationLifetime.ApplicationStopping.Register(async () =>
				{
					OnStopping();
					await discovery.DeregisterAsync(configure.DiscoveryClientConfiguration, configure.DiscoveryServiceConfiguration, configure.ServerPort);
					await server.ShutdownAsync();
					OnStopped();

				});

				server.Start();

				Logger.LogInformation("---------------> Grpc server has started");

				if (configure.DiscoveryServiceConfiguration != null)
				{
					Logger.LogInformation("---------------> Start registering consul service...");

					discovery.RegisterAsync(configure.DiscoveryClientConfiguration, configure.DiscoveryServiceConfiguration, configure.ServerPort, configure.Weight).GetAwaiter().GetResult();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "--------------->  Startup service has an error:{0}", ex.Message);
				throw;
			}
		}

		public void OnStopping()
		{

		}

		public void OnStopped()
		{

		}
	}
}
