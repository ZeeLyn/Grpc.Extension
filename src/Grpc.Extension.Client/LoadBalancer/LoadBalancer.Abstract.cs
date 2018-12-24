using System;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalancer
{
	public abstract class ILoadBalancer
	{
		public static Type Polling { get; set; } = typeof(LoadBalancerPolling);

		public abstract Channel GetChannel(string serviceName);
	}
}
