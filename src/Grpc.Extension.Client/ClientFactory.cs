using Grpc.Core.Interceptors;
using Grpc.Extension.Client.CircuitBreaker;
using Grpc.Extension.Client.LoadBalancer;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.Logging;


namespace Grpc.Extension.Client
{
	internal class ClientFactory : IClientFactory
	{

		private ILoadBalancer GrpcLoadBalance { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private CircuitBreakerPolicy CircuitBreakerPolicy { get; }

		private CircuitBreakerServiceBuilder CircuitBreakerServiceBuilder { get; }

		private ILogger Logger { get; }


		public ClientFactory(ILoadBalancer grpcLoadBalance, GrpcClientConfiguration grpcClientConfiguration,
			CircuitBreakerPolicy circuitBreakerPolicy, CircuitBreakerServiceBuilder circuitBreakerServiceBuilder,
			ILogger<CircuitBreakerInterceptor> logger)
		{
			GrpcLoadBalance = grpcLoadBalance;
			GrpcClientConfiguration = grpcClientConfiguration;
			CircuitBreakerPolicy = circuitBreakerPolicy;
			CircuitBreakerServiceBuilder = circuitBreakerServiceBuilder;
			Logger = logger;
		}


		public TService GetClient<TService>(string serviceName) where TService : IService<TService>
		{
			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			var caller = channel.Intercept(new CircuitBreakerInterceptor(CircuitBreakerPolicy, CircuitBreakerServiceBuilder, typeof(TService), GrpcClientConfiguration.CircuitBreakerOption, Logger));
			return MagicOnionClient.Create<TService>(caller);
		}

		public TStreamingHub GetStreamingHubClient<TStreamingHub, TReceiver>(string serviceName, TReceiver receiver) where TStreamingHub : IStreamingHub<TStreamingHub, TReceiver>
		{
			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			var caller = channel.Intercept(new CircuitBreakerInterceptor(CircuitBreakerPolicy, CircuitBreakerServiceBuilder, typeof(TStreamingHub), GrpcClientConfiguration.CircuitBreakerOption, Logger));
			return StreamingHubClient.Connect<TStreamingHub, TReceiver>(caller, receiver);
		}
	}
}
