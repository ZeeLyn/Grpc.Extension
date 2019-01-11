using System;
using Consul;

namespace Grpc.Extension.Core
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
