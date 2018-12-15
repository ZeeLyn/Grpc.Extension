using System;
using System.Collections.Generic;

namespace Grpc.Extension.Client
{
	public class GrpcClientConfiguration
	{
		public TimeSpan ChannelStatusCheckInterval { get; set; } = TimeSpan.FromSeconds(15);

		internal Type GrpcLoadBalancing { get; set; } = typeof(GrpcLoadBalancing);

		internal List<string> GrpcServiceName { get; set; } = new List<string>();
	}
}
