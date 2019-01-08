using System;
using System.Collections.Generic;

namespace Grpc.Extension.Server.ServiceDiscovery
{
	public interface IServiceConfiguration
	{
	}

	public class ConsulServiceConfiguration : IServiceConfiguration
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