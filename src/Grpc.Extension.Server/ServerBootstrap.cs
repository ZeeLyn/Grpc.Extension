using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Consul;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extension.Server.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Server
{
	public class ServerBootstrap : IDisposable
	{
		private CancellationTokenSource CancellationTokenSource { get; }

		private ILogger Logger { get; }

		public ServerBootstrap(ILogger<ServerBootstrap> logger)
		{
			CancellationTokenSource = new CancellationTokenSource();
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

				var server = new Core.Server
				{
					Ports = { configure.ServerPort }
				};

				//Register grpc service
				foreach (var service in configure.Services)
				{
					var bindMethod = service.Key.BaseType?.DeclaringType?.GetMethods().FirstOrDefault(p =>
						p.Name == "BindService" && p.ReturnType == typeof(ServerServiceDefinition) && p.IsPublic);
					if (bindMethod == null)
						throw new InvalidOperationException($"Type {service.Key.Name} is not a grpc service");
					var serviceInstance = Activator.CreateInstance(service.Key);
					var binder = bindMethod.Invoke(null, new[] { serviceInstance }) as ServerServiceDefinition;
					var interceptors = service.Value.Select(p => (Interceptor)Activator.CreateInstance(p)).ToArray();
					server.Services.Add(binder.Intercept(new DependencyInjectionInterceptor(app.ApplicationServices))
						.Intercept(interceptors));
				}

				//Register health check service
				server.Services.Add(Health.V1.Health.BindService(app.ApplicationServices.GetService<HealthCheckService.HealthCheckService>()));



				//Stop service
				applicationLifetime.ApplicationStopping.Register(async () =>
			   {
				   CancellationTokenSource.Cancel();
				   await discovery.DeregisterAsync(configure.DiscoveryClientConfiguration, configure.DiscoveryServiceConfiguration);
				   await server.ShutdownAsync();
			   });

				server.Start();

				Logger.LogInformation("---------------> Grpc server has started");

				if (configure.DiscoveryServiceConfiguration != null)
				{
					Logger.LogInformation("---------------> Start registering consul service...");

					//if (configure.AgentServiceConfiguration.Check != null)
					//{
					//	app.Map("/grpc/server/health/check", builder =>
					//	{
					//		builder.Run(async handler =>
					//		{
					//			handler.Response.StatusCode = (int)HttpStatusCode.OK;
					//			await handler.Response.WriteAsync($"{{\"Status\":\"{HttpStatusCode.OK}\"}}");
					//		});
					//	});
					//}

					discovery.RegisterAsync(configure.DiscoveryClientConfiguration, configure.DiscoveryServiceConfiguration, configure.Weight).GetAwaiter().GetResult();

					//using (var consul = new ConsulClient(conf =>
					//{
					//	conf.Address = configure.DiscoveryClientConfiguration.Address;
					//	conf.Datacenter = configure.DiscoveryClientConfiguration.Datacenter;
					//	conf.Token = configure.DiscoveryClientConfiguration.Token;
					//	conf.WaitTime = configure.DiscoveryClientConfiguration.WaitTime;
					//}))
					//{
					//	//Register service to consul agent 
					//	if (configure.DiscoveryServiceConfiguration.Meta == null)
					//		configure.DiscoveryServiceConfiguration.Meta = new Dictionary<string, string>();
					//	configure.DiscoveryServiceConfiguration.Meta.Add("X-Weight", configure.Weight.ToString());
					//	var result = consul.Agent
					//		.ServiceRegister(configure.DiscoveryServiceConfiguration, CancellationTokenSource.Token)
					//		.GetAwaiter().GetResult();
					//	if (result.StatusCode != HttpStatusCode.OK)
					//	{
					//		Logger.LogError("--------------->  Registration service failed:{0}", result.StatusCode);
					//		throw new ConsulRequestException("Registration service failed.", result.StatusCode);
					//	}
					//	Logger.LogInformation("---------------> Consul service registration completed");
					//}
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
			CancellationTokenSource.Cancel();
		}
	}
}
