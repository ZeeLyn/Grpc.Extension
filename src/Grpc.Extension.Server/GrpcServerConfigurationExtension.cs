﻿using System;
using Consul;
using Grpc.Core;

namespace Grpc.Extension.Server
{
	public static class GrpcServerConfigurationExtension
	{
		public static GrpcServerConfiguration AddService(this GrpcServerConfiguration configure, params ServerServiceDefinition[] serverServiceDefinition)
		{
			configure.Services.AddRange(serverServiceDefinition);
			configure.Services.Add(Health.V1.Health.BindService(new HealthCheckService.HealthCheckService()));
			return configure;
		}

		public static GrpcServerConfiguration AddServerPort(this GrpcServerConfiguration configure, string host,
			int port, ServerCredentials serverCredentials)
		{
			configure.ServerPort = new ServerPort(host, port, serverCredentials);
			return configure;
		}

		public static GrpcServerConfiguration AddConsul(this GrpcServerConfiguration configure, Action<ConsulAgentServiceConfiguration> agentServiceBuilder, Action<ConsulClientConfiguration> clientBuilder)
		{
			var agent = new ConsulAgentServiceConfiguration();
			agentServiceBuilder?.Invoke(agent);
			configure.AgentServiceConfiguration = new AgentServiceRegistration
			{
				Address = agent.Address,
				Port = agent.Port,
				ID = agent.ServiceId,
				Name = agent.ServiceName,
				EnableTagOverride = agent.EnableTagOverride,
				Meta = agent.Meta,
				Tags = agent.Tags
			};
			if (agent.HealthCheck != null)
			{
				configure.AgentServiceConfiguration.Check = new AgentServiceCheck
				{
					DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
					Interval = TimeSpan.FromSeconds(10),
					Timeout = TimeSpan.FromSeconds(3),
					HTTP = $"http://{agent.HealthCheck.Value.Host}:{agent.HealthCheck.Value.Port}/grpc/server/health/check"
				};
			}

			var client = new ConsulClientConfiguration();
			clientBuilder?.Invoke(client);
			configure.ConsulClientConfiguration = client;
			return configure;
		}
	}
}
