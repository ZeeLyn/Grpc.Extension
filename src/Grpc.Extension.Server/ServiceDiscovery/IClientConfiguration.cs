using System;
using System.Collections.Generic;
using System.Text;
using Consul;

namespace Grpc.Extension.Server.ServiceDiscovery
{
	public interface IClientConfiguration
	{
	}

	public class ConsulConfiguration : ConsulClientConfiguration, IClientConfiguration
	{
		public ConsulConfiguration()
		{
		}

		public ConsulConfiguration(string address)
		{
			Address = new Uri(address);
		}

		public ConsulConfiguration(string address, string token)
		{
			Address = new Uri(address);
			Token = token;
		}
	}
}
