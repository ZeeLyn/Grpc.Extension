using System;
using System.Linq;
using System.Net;
using System.Threading;
using Consul;
using Grpc.Core;
using Grpc.Core.Interceptors;
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

				Logger.LogInformation("---------------> Grpc server is starting...");

				var server = new Core.Server
				{
					Ports = { configure.ServerPort }
				};

				//Register grpc service
				foreach (var service in configure.Services)
				{
					var bindMethod = service.BaseType?.DeclaringType?.GetMethods().FirstOrDefault(p =>
						p.Name == "BindService" && p.ReturnType == typeof(ServerServiceDefinition) && p.IsPublic);
					if (bindMethod == null)
						throw new InvalidOperationException($"Type {service.Name} is not a grpc service");
					var serviceInstance = Activator.CreateInstance(service);
					var binder = bindMethod.Invoke(null, new[] { serviceInstance }) as ServerServiceDefinition;
					server.Services.Add(binder.Intercept(new DependencyInjectionInterceptor(app.ApplicationServices)));
				}
				//Register health check service
				server.Services.Add(Health.V1.Health.BindService(app.ApplicationServices.GetService<HealthCheckService.HealthCheckService>()));

				//Stop service
				applicationLifetime.ApplicationStopping.Register(() =>
				{
					CancellationTokenSource.Cancel();
					server.ShutdownAsync().GetAwaiter().GetResult();
					using (var consul = new ConsulClient(conf =>
					{
						conf.Address = configure.ConsulClientConfiguration.Address;
					}))
					{
						consul.Agent.ServiceDeregister(configure.AgentServiceConfiguration.ID)
							.GetAwaiter().GetResult();
					}

				});

				server.Start();

				Logger.LogInformation("---------------> Grpc server has started");

				if (configure.AgentServiceConfiguration != null)
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

					using (var consul = new ConsulClient(conf =>
					{
						conf.Address = configure.ConsulClientConfiguration.Address;
						conf.Datacenter = configure.ConsulClientConfiguration.Datacenter;
						conf.Token = configure.ConsulClientConfiguration.Token;
						conf.WaitTime = configure.ConsulClientConfiguration.WaitTime;
					}))
					{
						//Register service to consul agent 
						var result = consul.Agent
							.ServiceRegister(configure.AgentServiceConfiguration, CancellationTokenSource.Token)
							.GetAwaiter().GetResult();
						if (result.StatusCode != HttpStatusCode.OK)
						{
							Logger.LogError("--------------->  Registration service failed:{0}", result.StatusCode);
							throw new ConsulRequestException("Registration service failed.", result.StatusCode);
						}
						Logger.LogInformation("---------------> Consul service registration completed");
					}
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
