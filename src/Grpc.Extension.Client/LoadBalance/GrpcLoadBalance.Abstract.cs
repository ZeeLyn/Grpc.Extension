using System;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalance
{
	public abstract class GrpcLoadBalance
	{
		public static Type RoundRobin { get; set; } = typeof(GrpcLoadBalanceRound);

		public abstract Channel GetChannel(string serviceName);
	}
}
