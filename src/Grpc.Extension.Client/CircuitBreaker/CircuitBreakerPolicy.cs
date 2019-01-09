using System.Collections.Concurrent;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class CircuitBreakerPolicy
	{
		private static readonly ConcurrentDictionary<string, Policy> Policies = new ConcurrentDictionary<string, Policy>();

		private CircuitBreakerOption CircuitBreakerOption { get; }

		public CircuitBreakerPolicy(GrpcClientConfiguration grpcClientConfiguration)
		{
			CircuitBreakerOption = grpcClientConfiguration.CircuitBreakerOption;
		}

		public Policy<TResponse> GetOrCreatePolicy<TResponse>(string key)
		{
			if (CircuitBreakerOption == null)
				throw new InvalidOperationException("");
			//return Policies.GetOrAdd(key, k =>
			//{


			Policy<TResponse> policy = Policy<TResponse>.Handle<BrokenCircuitException>()
			.Or<TimeoutRejectedException>().Fallback(() => default(TResponse));
			//	.Fallback(
			//(ex, ctx, ct) => {
			//	CircuitBreakerOption.OnFallback?.Invoke(ex, ctx, ct);
			//},
			//(ex, ctx) => { CircuitBreakerOption.OnFallbackBefore?.Invoke(ex, ctx); });



			if (CircuitBreakerOption.InvokeTimeout.Ticks > 0)
			{
				policy = policy.Wrap(Policy.Timeout(CircuitBreakerOption.InvokeTimeout, TimeoutStrategy.Pessimistic));
			}


			if (CircuitBreakerOption.ExceptionsAllowedBeforeBreaking > 0)
			{
				policy = policy.Wrap(Policy<TResponse>.Handle<TimeoutRejectedException>().Or<Exception>()
					.CircuitBreaker(CircuitBreakerOption.ExceptionsAllowedBeforeBreaking,
						CircuitBreakerOption.DurationOfBreak));
				//.CircuitBreaker(
				//CircuitBreakerOption.ExceptionsAllowedBeforeBreaking, CircuitBreakerOption.DurationOfBreak,
				//(ex, ts, ctx) => { CircuitBreakerOption.OnBreak?.Invoke(ex, ts, ctx); },
				//ctx => { CircuitBreakerOption.OnReset?.Invoke(ctx); }));
			}

			return policy;
			//});
		}
	}
}
