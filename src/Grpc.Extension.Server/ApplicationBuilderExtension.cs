using System;
using System.Linq;
using System.Net;
using System.Threading;
using Consul;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public static class ApplicationBuilderExtension
	{
		private static CancellationTokenSource CancellationTokenSource { get; }

		static ApplicationBuilderExtension()
		{
			CancellationTokenSource = new CancellationTokenSource();
		}


		public static IApplicationBuilder UseGrpcServer(this IApplicationBuilder app)
		{
			var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
			var configure = app.ApplicationServices.GetService<GrpcServerConfiguration>();
			if (configure.ServerPort == null)
				throw new ArgumentNullException(nameof(configure.ServerPort));

			var server = new Core.Server
			{
				Ports = { configure.ServerPort }
			};

			foreach (var service in configure.Services)
			{
				var bindMethod = service.BaseType?.DeclaringType?.GetMethods().FirstOrDefault(p =>
					p.Name == "BindService" && p.ReturnType == typeof(ServerServiceDefinition) && p.IsPublic);
				if (bindMethod == null)
					throw new InvalidOperationException($"Type {service.Name} is not a grpc service");
				var serviceInstance = Activator.CreateInstance(service);
				var binder = bindMethod.Invoke(null, new[] { serviceInstance }) as ServerServiceDefinition;
				server.Services.Add(binder.Intercept(new DependencyInjectionInterceptor(app.ApplicationServices, serviceInstance)));
			}

			server.Services.Add(Health.V1.Health.BindService(new HealthCheckService.HealthCheckService()));

			applicationLifetime.ApplicationStopping.Register(() =>
			{
				CancellationTokenSource.Cancel();
				using (var consul = new ConsulClient(conf => { conf.Address = configure.ConsulClientConfiguration.Address; }))
				{
					consul.Agent.ServiceDeregister(configure.AgentServiceConfiguration.ID)
						.GetAwaiter().GetResult();
				}

				server.ShutdownAsync().GetAwaiter().GetResult();
			});

			server.Start();

			if (configure.AgentServiceConfiguration != null)
			{
				if (configure.AgentServiceConfiguration.Check != null)
				{
					app.Map("/grpc/server/health/check", builder =>
					{
						builder.Run(async handler =>
						{
							handler.Response.StatusCode = (int)HttpStatusCode.OK;
							await handler.Response.WriteAsync($"{{\"Status\":\"{HttpStatusCode.OK}\"}}");
						});
					});
				}

				using (var consul = new ConsulClient(conf =>
				{
					conf.Address = configure.ConsulClientConfiguration.Address;
					conf.Datacenter = configure.ConsulClientConfiguration.Datacenter;
					conf.Token = configure.ConsulClientConfiguration.Token;
					conf.WaitTime = configure.ConsulClientConfiguration.WaitTime;
				}))
				{
					var result = consul.Agent
						.ServiceRegister(configure.AgentServiceConfiguration, CancellationTokenSource.Token)
						.GetAwaiter().GetResult();
					if (result.StatusCode != HttpStatusCode.OK)
					{
						throw new ConsulRequestException("Registration service failed.", result.StatusCode);
					}
				}
			}

			return app;
		}
	}
}
