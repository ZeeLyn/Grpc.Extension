using System;
using System.Collections.Generic;
using Consul;
using Grpc.Core;
using Grpc.Extension.Server.ServiceDiscovery;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public class GrpcServerConfiguration
	{
		internal IServiceCollection ServiceCollection { get; set; }
		internal ServerPort ServerPort { get; set; }

		internal IServiceConfiguration DiscoveryServiceConfiguration { get; set; }

		internal IClientConfiguration DiscoveryClientConfiguration { get; set; }

		//internal Type ServiceDiscovery { get; set; }

		internal Dictionary<Type, List<Type>> Services { get; set; } = new Dictionary<Type, List<Type>>();

		internal int? Weight { get; set; }
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

		public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(10);
	}

}
