using System;
using System.Threading;
using Consul;
using Microsoft.Extensions.Hosting;

namespace Grpc.Extension.Server
{
	public static class HostExtension
	{
		//private static CancellationTokenSource CancellationTokenSource { get; }

		//static HostExtension()
		//{
		//	CancellationTokenSource = new CancellationTokenSource();
		//}

		//public static IHost UseGrpcServer(this IHost host)
		//{
		//var applicationLifetime =
		//	(IApplicationLifetime)host.Services.GetService(typeof(IApplicationLifetime));

		//	var configure = (GrpcServerConfiguration)host.Services.GetService(typeof(GrpcServerConfiguration));

		//	var server = new Core.Server
		//	{
		//		Ports = { configure.ServerPort }
		//	};
		//	foreach (var service in configure.Services)
		//	{
		//		server.Services.Add(service);
		//	}

		//	applicationLifetime.ApplicationStopping.Register(() =>
		//	{

		//		using (var consul = new ConsulClient(configure.ConsulClientConfigurationAction))
		//		{
		//			consul.Agent.ServiceDeregister(configure.ServiceConfiguration.ID).GetAwaiter().GetResult();
		//		}
		//		server.ShutdownAsync().GetAwaiter().GetResult();
		//		CancellationTokenSource.Cancel();
		//	});

		//	server.Start();

		//	if (configure.ConsulClientConfigurationAction != null)
		//	{
		//		using (var consul = new ConsulClient(configure.ConsulClientConfigurationAction))
		//		{
		//			var result = consul.Agent
		//				.ServiceRegister(configure.ServiceConfiguration, CancellationTokenSource.Token)
		//				.GetAwaiter().GetResult();
		//			if (result.StatusCode != System.Net.HttpStatusCode.OK)
		//			{
		//				throw new ConsulRequestException("Registration service failed.", result.StatusCode);
		//			}
		//		}
		//	}

		//	return host;
		//}



	}
}

