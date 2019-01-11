using System.Collections.Concurrent;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Core;
using MagicOnion;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class CircuitBreakerPolicy
	{
		private static readonly ConcurrentDictionary<string, object> Policies = new ConcurrentDictionary<string, object>();

		private CircuitBreakerOption CircuitBreakerOption { get; }

		private CircuitBreakerServiceBuilder CircuitBreakerServiceBuilder { get; }

		private IServiceInjectionCommand ServiceInjectionCommand { get; }

		public CircuitBreakerPolicy(GrpcClientConfiguration grpcClientConfiguration, CircuitBreakerServiceBuilder circuitBreakerServiceBuilder, IServiceInjectionCommand serviceInjectionCommand)
		{
			CircuitBreakerOption = grpcClientConfiguration.CircuitBreakerOption;
			CircuitBreakerServiceBuilder = circuitBreakerServiceBuilder;
			ServiceInjectionCommand = serviceInjectionCommand;
		}

		public Policy<TResponse> GetOrCreatePolicyForSyncInvoker<TResponse>(string key)
		{
			if (CircuitBreakerOption == null)
				throw new InvalidOperationException("");
			//return Policies.GetOrAdd(key, k =>
			//{


			Policy<TResponse> policy = Policy<TResponse>.Handle<BrokenCircuitException>()
			.Or<TimeoutRejectedException>().Fallback(() =>
			{
				Console.WriteLine("timeout...........");
				return default(TResponse);
			});
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

		public Policy<AsyncUnaryCall<TResponse>> GetOrCreatePolicyForAsyncUnaryCall<TResponse>(Type serviceType, string key)
		{
			if (CircuitBreakerOption == null)
				throw new InvalidOperationException("");
			return (Policy<AsyncUnaryCall<TResponse>>)Policies.GetOrAdd(key, k =>
			{
				Policy<AsyncUnaryCall<TResponse>> policy = Policy<AsyncUnaryCall<TResponse>>
					.Handle<BrokenCircuitException>()
					.Or<TimeoutRejectedException>().Or<Exception>().Fallback(cl =>
					{
						var command = ServiceInjectionCommand.GetCommand(serviceType, key);
						return new AsyncUnaryCall<TResponse>(
							Task.Run(() => (TResponse)ServiceInjectionCommand.Run(command.Command, command.Namespace).GetAwaiter().GetResult(), cl),
							Task.Run(() => new Metadata(), cl), () => new Status(StatusCode.OK, ""),
							() => new Metadata(), () => { });
					});

				if (CircuitBreakerOption.InvokeTimeout.Ticks > 0)
				{
					policy = policy.Wrap(Policy.Timeout(CircuitBreakerOption.InvokeTimeout, TimeoutStrategy.Pessimistic));
				}

				if (CircuitBreakerOption.ExceptionsAllowedBeforeBreaking > 0)
				{
					policy = policy.Wrap(Policy<AsyncUnaryCall<TResponse>>.Handle<TimeoutRejectedException>().Or<Exception>()
						.CircuitBreaker(CircuitBreakerOption.ExceptionsAllowedBeforeBreaking,
							CircuitBreakerOption.DurationOfBreak));
				}

				return policy;
			});
		}
	}
}
