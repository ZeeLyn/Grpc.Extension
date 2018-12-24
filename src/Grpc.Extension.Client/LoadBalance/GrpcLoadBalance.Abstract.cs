using System;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalance
{
	public abstract class GrpcLoadBalance
	{
		public static Type Polling { get; set; } = typeof(GrpcLoadBalancePolling);

		public abstract Channel GetChannel(string serviceName);
	}
}
