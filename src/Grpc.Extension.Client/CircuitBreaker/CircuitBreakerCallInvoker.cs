using Grpc.Core;
using Grpc.Core.Utils;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class CircuitBreakerCallInvoker : CallInvoker
	{
		private Channel Channel { get; }
		private CircuitBreakerPolicy CircuitBreakerPolicy { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }


		public CircuitBreakerCallInvoker(Channel channel, CircuitBreakerPolicy circuitBreakerPolicy, GrpcClientConfiguration grpcClientConfiguration)
		{
			Channel = GrpcPreconditions.CheckNotNull(channel);
			CircuitBreakerPolicy = circuitBreakerPolicy;
			GrpcClientConfiguration = grpcClientConfiguration;
		}

		public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
		{
			var call = CreateCall(method, host, options);
			if (GrpcClientConfiguration.CircuitBreakerOption != null)
			{
				var policy = CircuitBreakerPolicy.GetOrCreatePolicy<TResponse>($"{call.Channel.Target}/{call.Method}");
				return policy.Execute(() => Calls.BlockingUnaryCall(call, request));
			}
			return Calls.BlockingUnaryCall(call, request);
		}

		public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
		{
			var call = CreateCall(method, host, options);
			//if (GrpcClientConfiguration.CircuitBreakerOption != null)
			//{
			//	var policy = CircuitBreakerPolicy.GetOrCreatePolicy($"{call.Channel.Target}/{call.Method}");
			//	return policy.Execute(() => Calls.AsyncUnaryCall(call, request));
			//}
			return Calls.AsyncUnaryCall(call, request);
		}

		public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options,
			TRequest request)
		{
			var call = CreateCall(method, host, options);
			return Calls.AsyncServerStreamingCall(call, request);
		}

		public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
		{
			var call = CreateCall(method, host, options);
			return Calls.AsyncClientStreamingCall(call);
		}

		public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
		{
			var call = CreateCall(method, host, options);
			return Calls.AsyncDuplexStreamingCall(call);
		}

		protected virtual CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
			where TRequest : class
			where TResponse : class
		{
			return new CallInvocationDetails<TRequest, TResponse>(Channel, method, host, options);
		}
	}
}
