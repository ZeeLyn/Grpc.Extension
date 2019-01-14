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

		public Policy<TResponse> GetOrCreatePolicyForUnaryCall<TResponse>(Type serviceType, string key)
		{
			if (CircuitBreakerOption == null)
				throw new InvalidOperationException("");
			return (Policy<TResponse>)Policies.GetOrAdd(key, k =>
			{
				Policy<TResponse> policy = Policy<TResponse>
					.Handle<BrokenCircuitException>()
					.Or<TimeoutRejectedException>().Or<Exception>().Fallback(cl =>
					{
						var command = ServiceInjectionCommand.GetCommand(serviceType, key);
						return (TResponse)ServiceInjectionCommand.Run(command.Command, command.Namespace).GetAwaiter()
							.GetResult();
					});

				if (CircuitBreakerOption.InvokeTimeout.Ticks > 0)
				{
					policy = policy.Wrap(Policy.Timeout(CircuitBreakerOption.InvokeTimeout, TimeoutStrategy.Pessimistic));
				}

				//if (CircuitBreakerOption.Retry > 0)
				//{
				//	policy = policy.Wrap(Policy.Handle<BrokenCircuitException>()
				//		.Or<TimeoutRejectedException>().Or<Exception>().Retry(CircuitBreakerOption.Retry));
				//}

				if (CircuitBreakerOption.ExceptionsAllowedBeforeBreaking > 0)
				{
					policy = policy.Wrap(Policy<TResponse>.Handle<TimeoutRejectedException>().Or<Exception>()
						.CircuitBreaker(CircuitBreakerOption.ExceptionsAllowedBeforeBreaking,
							CircuitBreakerOption.DurationOfBreak, (r, ts) => { Console.WriteLine("breaker------------------------"); }, () => { }));
				}

				return policy;
			});
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

				//if (CircuitBreakerOption.Retry > 0)
				//{
				//	policy = policy.Wrap(Policy.Handle<BrokenCircuitException>()
				//		.Or<TimeoutRejectedException>().Or<Exception>().Retry(CircuitBreakerOption.Retry));
				//}

				if (CircuitBreakerOption.ExceptionsAllowedBeforeBreaking > 0)
				{
					policy = policy.Wrap(Policy<AsyncUnaryCall<TResponse>>.Handle<TimeoutRejectedException>().Or<Exception>()
						.CircuitBreaker(CircuitBreakerOption.ExceptionsAllowedBeforeBreaking,
							CircuitBreakerOption.DurationOfBreak, (r, ts) => { Console.WriteLine("breaker------------------------"); }, () => { }));
				}

				return policy;
			});
		}

		public Policy<AsyncClientStreamingCall<TRequest, TResponse>> GetOrCreatePolicyForAsyncClientStreamingCall<TRequest, TResponse>(Type serviceType, string key)
		{
			if (CircuitBreakerOption == null)
				throw new InvalidOperationException("");
			return (Policy<AsyncClientStreamingCall<TRequest, TResponse>>)Policies.GetOrAdd(key, k =>
			{
				Policy<AsyncClientStreamingCall<TRequest, TResponse>> policy = Policy<AsyncClientStreamingCall<TRequest, TResponse>>
					.Handle<BrokenCircuitException>()
					.Or<TimeoutRejectedException>().Or<Exception>().Fallback(cl =>
					{
						//var command = ServiceInjectionCommand.GetCommand(serviceType, key);

						return new AsyncClientStreamingCall<TRequest, TResponse>(null, null, null, null, null, null);
						//return new AsyncUnaryCall<TResponse>(
						//	Task.Run(() => (TResponse)ServiceInjectionCommand.Run(command.Command, command.Namespace).GetAwaiter().GetResult(), cl),
						//	Task.Run(() => new Metadata(), cl), () => new Status(StatusCode.OK, ""),
						//	() => new Metadata(), () => { });
					});

				if (CircuitBreakerOption.InvokeTimeout.Ticks > 0)
				{
					policy = policy.Wrap(Policy.Timeout(CircuitBreakerOption.InvokeTimeout, TimeoutStrategy.Pessimistic));
				}

				//if (CircuitBreakerOption.Retry > 0)
				//{
				//	policy = policy.Wrap(Policy.Handle<BrokenCircuitException>()
				//		.Or<TimeoutRejectedException>().Or<Exception>().Retry(CircuitBreakerOption.Retry));
				//}

				if (CircuitBreakerOption.ExceptionsAllowedBeforeBreaking > 0)
				{
					policy = policy.Wrap(Policy<AsyncClientStreamingCall<TRequest, TResponse>>.Handle<TimeoutRejectedException>().Or<Exception>()
						.CircuitBreaker(CircuitBreakerOption.ExceptionsAllowedBeforeBreaking,
							CircuitBreakerOption.DurationOfBreak, (r, ts) => { Console.WriteLine("breaker------------------------"); }, () => { }));
				}

				return policy;
			});
		}
	}
}
