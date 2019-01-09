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

		internal Dictionary<Type, List<Type>> Services { get; set; } = new Dictionary<Type, List<Type>>();

		internal int? Weight { get; set; }
	}
}
