using System;
using System.Collections.Generic;
using Consul;
using Grpc.Core;

namespace Grpc.Extension.Server
{
	public class GrpcServerConfiguration
	{
		internal ServerPort ServerPort { get; set; }

		internal AgentServiceRegistration AgentServiceConfiguration { get; set; }

		internal ConsulClientConfiguration ConsulClientConfiguration { get; set; }

		internal List<Type> Services { get; set; } = new List<Type>();

	}

	public class ConsulAgentServiceConfiguration
	{
		public string Address { get; set; }

		public int Port { get; set; }

		public string ServiceId { get; set; }

		public string ServiceName { get; set; }

		public bool EnableTagOverride { get; set; }

		public Dictionary<string, string> Meta { get; set; }

		public string[] Tags { get; set; }

		//public (string Host, int Port)? HealthCheck { get; set; }

		public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(10);
	}

}
