using System;
using System.Net;
using System.Threading;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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
			var applicationLifetime =
				(IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));

			var configure =
				(GrpcServerConfiguration)app.ApplicationServices.GetService(typeof(GrpcServerConfiguration));

			if (configure.ServerPort == null)
				throw new ArgumentNullException(nameof(configure.ServerPort));

			var server = new Core.Server
			{
				Ports = { configure.ServerPort }
			};
			foreach (var service in configure.Services)
			{
				server.Services.Add(service);
			}

			applicationLifetime.ApplicationStopping.Register(() =>
			{
				CancellationTokenSource.Cancel();
				using (var consul = new ConsulClient(conf => { conf.Address = configure.ConsulClientConfiguration.Address; }))
				{

					consul.Agent.ServiceDeregister(configure.AgentServiceConfiguration.ID, CancellationTokenSource.Token)
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
