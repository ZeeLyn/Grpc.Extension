using System;
using Grpc.Core;
using Grpc.Extension.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public static class GrpcServerConfigurationExtension
	{
		public static GrpcServerConfiguration AddServerPort(this GrpcServerConfiguration configure, string host,
			int port, ServerCredentials serverCredentials)
		{
			configure.ServerPort = new ServerPort(host, port, serverCredentials);
			return configure;
		}
		public static GrpcServerConfiguration AddServerPort(this GrpcServerConfiguration configure, ServerPort serverPort)
		{
			configure.ServerPort = serverPort;
			return configure;
		}




		public static GrpcServerConfiguration AddServiceDiscovery<TDiscovery>(this GrpcServerConfiguration grpcClientConfiguration, IClientConfiguration clientConfiguration, IServiceConfiguration serviceConfiguration, int? weight = default) where TDiscovery : IServiceDiscovery
		{
			if (weight.HasValue)
			{
				if (weight.Value <= 0)
					throw new ArgumentException("The weighted value must be greater than 0");
				grpcClientConfiguration.Weight = weight.Value;
			}
			grpcClientConfiguration.ServiceCollection.AddSingleton(typeof(IServiceDiscovery), typeof(TDiscovery));
			grpcClientConfiguration.DiscoveryClientConfiguration = clientConfiguration;
			grpcClientConfiguration.DiscoveryServiceConfiguration = serviceConfiguration;
			return grpcClientConfiguration;
		}

		public static GrpcServerConfiguration AddConsulServiceDiscovery(
			this GrpcServerConfiguration grpcClientConfiguration, ConsulConfiguration clientConfiguration,
			ConsulServiceConfiguration serviceConfiguration, int? weight = default)
		{
			return grpcClientConfiguration.AddServiceDiscovery<ConsulServiceDiscovery>(clientConfiguration, serviceConfiguration, weight);
		}
	}
}
