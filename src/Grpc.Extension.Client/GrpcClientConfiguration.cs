using System;
using System.Collections.Generic;
using System.Threading;
using Consul;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalancer;
using Polly;
using Polly.CircuitBreaker;

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

		internal CircuitBreakerOption CircuitBreakerOption { get; set; }
	}


	public class CircuitBreakerOption
	{
		/// <summary>
		/// The number of exceptions or handled results that are allowed before opening the circuit.
		/// </summary>
		public int ExceptionsAllowedBeforeBreaking { get; set; } = 10;

		/// <summary>
		/// The duration the circuit will stay open before resetting.
		/// </summary>
		public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromMinutes(1);

		/// <summary>
		/// Invokes timeout
		/// </summary>
		public TimeSpan InvokeTimeout { get; set; }

		/// <summary>
		/// Filtered exception type
		/// </summary>
		public IEnumerable<Type> Exceptions { get; set; } = new List<Type> { };

		/// <summary>
		/// The exception predicate
		/// </summary>
		public Func<Exception, bool> ExceptionPredicate { get; set; }

		public Action<Exception, Context, CancellationToken> OnFallback { get; set; }

		/// <summary>
		/// The action to call asynchronously before invoking the fallback delegate.
		/// </summary>
		public Action<Exception, Context> OnFallbackBefore { get; set; }

		/// <summary>
		/// The action to call when the circuit transitions to an <see cref="CircuitState.Open" /> state.
		/// </summary>
		public Action<Exception, TimeSpan, Context> OnBreak { get; set; }

		/// <summary>
		/// The action to call when the circuit resets to a <see cref="CircuitState.Closed" /> state.
		/// </summary>
		public Action<Context> OnReset { get; set; }
	}
}
