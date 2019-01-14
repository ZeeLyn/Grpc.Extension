using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Extension.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class CircuitBreakerInterceptor : Interceptor
	{
		private CircuitBreakerPolicy CircuitBreakerPolicy { get; }
		private CircuitBreakerServiceBuilder CircuitBreakerServiceBuilder { get; }

		private CircuitBreakerOption CircuitBreakerOption { get; }

		private Type ServiceType { get; }

		private ILogger Logger { get; }

		public CircuitBreakerInterceptor(CircuitBreakerPolicy circuitBreakerPolicy, CircuitBreakerServiceBuilder circuitBreakerServiceBuilder, Type serviceType, CircuitBreakerOption circuitBreakerOption, ILogger logger)
		{
			CircuitBreakerPolicy = circuitBreakerPolicy;
			CircuitBreakerServiceBuilder = circuitBreakerServiceBuilder;
			ServiceType = serviceType;
			CircuitBreakerOption = circuitBreakerOption;
			Logger = logger;
		}


		public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			Logger.LogInformation("Exec BlockingUnaryCall Interceptor----------> Metod name:{0}", context.Method.FullName);
			if (IsEnableCircuitBreaker(context.Method.FullName))
			{
				var policy =
					CircuitBreakerPolicy.GetOrCreatePolicyForUnaryCall<TResponse>(ServiceType, $"{context.Method.FullName}");
				return policy.Execute(() =>
				{

					return base.BlockingUnaryCall(request, context, continuation);
				});
			}
			return base.BlockingUnaryCall(request, context, continuation);
		}



		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
		{
			Logger.LogInformation("Exec AsyncUnaryCall Interceptor----------> Metod name:{0}", context.Method.FullName);
			if (IsEnableCircuitBreaker(context.Method.FullName))
			{
				var policy =
					CircuitBreakerPolicy.GetOrCreatePolicyForAsyncUnaryCall<TResponse>(ServiceType, $"{context.Method.FullName}");
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
