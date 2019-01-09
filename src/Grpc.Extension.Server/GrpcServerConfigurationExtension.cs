﻿using System;
using Grpc.Core;
using Grpc.Extension.Server.ServiceDiscovery;
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

		//public static GrpcServerConfiguration AddConsul(this GrpcServerConfiguration configure, Action<ConsulClientConfiguration> clientBuilder, Action<ConsulAgentServiceConfiguration> serviceBuilder)
		//{
		//	var agent = new ConsulAgentServiceConfiguration();
		//	serviceBuilder?.Invoke(agent);
		//	configure.ServiceDiscoveryServiceConfiguration = new AgentServiceRegistration
		//	{
		//		Address = agent.Address,
		//		Port = agent.Port,
		//		ID = string.IsNullOrWhiteSpace(agent.ServiceId) ? $"{agent.Address}:{agent.Port}" : agent.ServiceId,
		//		Name = string.IsNullOrWhiteSpace(agent.ServiceName) ? $"{agent.Address}:{agent.Port}" : agent.ServiceName,
		//		EnableTagOverride = agent.EnableTagOverride,
		//		Meta = agent.Meta,
		//		Tags = agent.Tags
		//	};
		//	if (agent.HealthCheckInterval.Ticks > 0)
		//	{
		//		configure.ServiceDiscoveryServiceConfiguration.Check = new AgentServiceCheck
		//		{
		//			DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
		//			Interval = agent.HealthCheckInterval,
		//			Timeout = TimeSpan.FromSeconds(3),
		//			GRPC = $"{agent.Address}:{agent.Port}"
		//		};
		//	}

		//	var client = new ConsulClientConfiguration();
		//	clientBuilder?.Invoke(client);
		//	configure.ServiceDiscoveryClientConfiguration = client;
		//	return configure;
		//}


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
	}
}
