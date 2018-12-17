using System;
using System.Collections.Generic;
using Consul;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalance;

namespace Grpc.Extension.Client
{
	public class GrpcClientConfiguration
	{
		public TimeSpan ChannelStatusCheckInterval { get; set; } = TimeSpan.FromSeconds(15);

		internal Type GrpcLoadBalance { get; set; } = LoadBalance.GrpcLoadBalance.RoundRobin;

		internal ConsulClientConfiguration ConsulClientConfiguration { get; set; }

		internal List<ServiceConfiguration> ServicesConfiguration { get; set; } = new List<ServiceConfiguration>();
	}

	public class ServiceConfiguration
	{
		public ServiceConfiguration()
		{
		}

		public ServiceConfiguration(string serviceName, ChannelCredentials channelCredentials)
		{
			ServiceName = serviceName;
			ChannelCredentials = channelCredentials;
		}

		public string ServiceName { get; set; }

		public ChannelCredentials ChannelCredentials { get; set; } = ChannelCredentials.Insecure;
	}
}
