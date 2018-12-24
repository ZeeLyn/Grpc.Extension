using System;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalancer
{
	public abstract class ILoadBalancer
	{
		public static Type Polling { get; set; } = typeof(LoadBalancerPolling);

		public static Type WeightedPolling { get; set; } = typeof(LoadBalancerWeightedPolling);

		public abstract Channel GetNextChannel(string serviceName);
	}
}
