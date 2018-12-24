using System;
using System.Collections.Generic;
using Consul;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalancer;

namespace Grpc.Extension.Client
{
	public class GrpcClientConfiguration
	{
		public TimeSpan ChannelStatusCheckInterval { get; set; } = TimeSpan.FromSeconds(15);

		internal Type GrpcLoadBalance { get; set; } = ILoadBalancer.Polling;

		internal ConsulClientConfiguration ConsulClientConfiguration { get; set; }

		internal Dictionary<string, ChannelCredentials> ServicesCredentials { get; set; } =
			new Dictionary<string, ChannelCredentials>();

		internal List<Type> ClientTypes { get; set; } = new List<Type>();
	}

}
