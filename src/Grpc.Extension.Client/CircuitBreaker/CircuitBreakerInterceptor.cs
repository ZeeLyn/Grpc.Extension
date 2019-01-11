using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extension.Core;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class CircuitBreakerInterceptor : Interceptor
	{

		private CircuitBreakerPolicy CircuitBreakerPolicy { get; }
		private CircuitBreakerServiceBuilder CircuitBreakerServiceBuilder { get; }

		private CircuitBreakerOption CircuitBreakerOption { get; }

		private Type ServiceType { get; }

		public CircuitBreakerInterceptor(CircuitBreakerPolicy circuitBreakerPolicy, CircuitBreakerServiceBuilder circuitBreakerServiceBuilder, Type serviceType, CircuitBreakerOption circuitBreakerOption)
		{
			CircuitBreakerPolicy = circuitBreakerPolicy;
			CircuitBreakerServiceBuilder = circuitBreakerServiceBuilder;
			ServiceType = serviceType;
			CircuitBreakerOption = circuitBreakerOption;
		}

		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
		{
			return await base.UnaryServerHandler(request, context, continuation);
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			if (IsEnableCircuitBreaker(context.Method.FullName))
			{
				var policy =
					CircuitBreakerPolicy.GetOrCreatePolicyForAsyncInvoker<TResponse>(ServiceType, $"{context.Method.FullName}");
				return policy.Execute(() =>
				{
					var task = base.AsyncUnaryCall(request, context, continuation);
					task.GetAwaiter().GetResult();
					return task;
				});
			}

			return base.AsyncUnaryCall(request, context, continuation);
		}

		private bool IsEnableCircuitBreaker(string serviceName)
		{
			var attr = CircuitBreakerServiceBuilder.GetAttribute<NonCircuitBreakerAttribute>(ServiceType, serviceName);
			return CircuitBreakerOption != null && attr == null;
		}
	}
}
