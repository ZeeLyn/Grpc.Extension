using System;
using System.Collections.Generic;
using System.Threading;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalancer;
using Grpc.Extension.Core;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;

namespace Grpc.Extension.Client
{
	public class GrpcClientConfiguration
	{
		internal IServiceCollection ServiceCollection { get; set; }
		public TimeSpan ChannelStatusCheckInterval { get; set; } = TimeSpan.FromSeconds(15);

		internal Type GrpcLoadBalance { get; set; } = ILoadBalancer.Polling;

		internal IClientConfiguration ConsulClientConfiguration { get; set; }

		internal Dictionary<string, ChannelCredentials> ServicesCredentials { get; set; } =
			new Dictionary<string, ChannelCredentials>();

		internal CircuitBreakerOption CircuitBreakerOption { get; set; }
	}


	public class CircuitBreakerOption
	{
		/// <summary>
		/// The number of exceptions or handled results that are allowed before opening the circuit.
		/// </summary>
		public int ExceptionsAllowedBeforeBreaking { get; set; } = 3;

		/// <summary>
		/// The duration the circuit will stay open before resetting.
		/// </summary>
		public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromMinutes(1);

		/// <summary>
		/// Invokes timeout
		/// </summary>
		public TimeSpan InvokeTimeout { get; set; }

		public int Retry { get; set; }
	}
}
