using System;
using System.Linq;
using Grpc.Core;
using Grpc.Extension.Client.CircuitBreaker;
using Grpc.Extension.Client.LoadBalancer;


namespace Grpc.Extension.Client
{
	public class ClientFactory
	{

		private ILoadBalancer GrpcLoadBalance { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private CircuitBreakerPolicy CircuitBreakerPolicy { get; }

		//private CircuitBreakerCallInvoker CircuitBreakerCallInvoker { get; }

		public ClientFactory(ILoadBalancer grpcLoadBalance, GrpcClientConfiguration grpcClientConfiguration, CircuitBreakerPolicy circuitBreakerPolicy)
		{
			GrpcLoadBalance = grpcLoadBalance;
			GrpcClientConfiguration = grpcClientConfiguration;
			CircuitBreakerPolicy = circuitBreakerPolicy;
			//CircuitBreakerCallInvoker = circuitBreakerCallInvoker;
		}


		public TGrpcClient Get<TGrpcClient>(string serviceName) where TGrpcClient : ClientBase
		{
			var type = GrpcClientConfiguration.ClientTypes.FirstOrDefault(p => p == typeof(TGrpcClient));
			if (type == null)
				throw new InvalidOperationException($"Not found client {typeof(TGrpcClient)}.");
			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			var call = new CircuitBreakerCallInvoker(channel, CircuitBreakerPolicy, GrpcClientConfiguration);
			return (TGrpcClient)Activator.CreateInstance(type, call);
		}
	}
}
