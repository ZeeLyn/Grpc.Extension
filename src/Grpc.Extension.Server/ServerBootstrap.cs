using System;
using System.Linq;
using System.Threading;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extension.Server.ServiceDiscovery;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Server
{
	public class ServerBootstrap : IDisposable
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

				var services = MagicOnionEngine.BuildServerServiceDefinition(true);
				var server = new Core.Server
				{
					Ports = { configure.ServerPort },
					Services = { services.ServerServiceDefinition.Intercept(new DependencyInjectionInterceptor(app.ApplicationServices)) }
				};

				//Register grpc service
				//foreach (var service in configure.Services)
				//{
				//	var bindMethod = service.Key.BaseType?.DeclaringType?.GetMethods().FirstOrDefault(p =>
				//		p.Name == "BindService" && p.ReturnType == typeof(ServerServiceDefinition) && p.IsPublic);
				//	if (bindMethod == null)
				//		throw new InvalidOperationException($"Type {service.Key.Name} is not a grpc service");
				//	var serviceInstance = Activator.CreateInstance(service.Key);
				//	var binder = bindMethod.Invoke(null, new[] { serviceInstance }) as ServerServiceDefinition;
				//	var interceptors = service.Value.Select(p => (Interceptor)Activator.CreateInstance(p)).ToArray();
				//	server.Services.Add(binder.Intercept(new DependencyInjectionInterceptor(app.ApplicationServices))
				//		.Intercept(interceptors));
				//}

				//Register health check service
				//server.Services.Add(Health.V1.Health.BindService(app.ApplicationServices.GetService<HealthCheckService.HealthCheckService>()));



				//Stop service
				applicationLifetime.ApplicationStopping.Register(async () =>
			   {
				   await discovery.DeregisterAsync(configure.DiscoveryClientConfiguration, configure.DiscoveryServiceConfiguration);
				   await server.ShutdownAsync();
			   });

				server.Start();

				Logger.LogInformation("---------------> Grpc server has started");

				if (configure.DiscoveryServiceConfiguration != null)
				{
					Logger.LogInformation("---------------> Start registering consul service...");

					discovery.RegisterAsync(configure.DiscoveryClientConfiguration, configure.DiscoveryServiceConfiguration, configure.Weight).GetAwaiter().GetResult();
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "--------------->  Startup service has an error:{0}", ex.Message);
				throw;
			}
		}

		public void Dispose()
		{
		}
	}
}
