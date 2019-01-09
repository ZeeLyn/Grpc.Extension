using System;
using System.Linq;
using Grpc.Core;
using Grpc.Extension.Client.CircuitBreaker;
using Grpc.Extension.Client.LoadBalancer;
using MagicOnion;
using MagicOnion.Client;


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


		public TService Get<TService>(string serviceName) where TService : IService<TService>
		{
			//var type = GrpcClientConfiguration.ClientTypes.FirstOrDefault(p => p == typeof(TGrpcClient));
			//if (type == null)
			//	throw new InvalidOperationException($"Not found client {typeof(TGrpcClient)}.");
			//var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			//var call = new CircuitBreakerCallInvoker(channel, CircuitBreakerPolicy, GrpcClientConfiguration);
			//return (TGrpcClient)Activator.CreateInstance(type, call);

			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			var caller = new CircuitBreakerCallInvoker(channel, CircuitBreakerPolicy, GrpcClientConfiguration);
			return MagicOnionClient.Create<TService>(caller);
		}
	}
}
