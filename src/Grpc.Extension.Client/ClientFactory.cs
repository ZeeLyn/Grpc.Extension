using Grpc.Core.Interceptors;
using Grpc.Extension.Client.CircuitBreaker;
using Grpc.Extension.Client.LoadBalancer;
using MagicOnion;
using MagicOnion.Client;


namespace Grpc.Extension.Client
{
	internal class ClientFactory : IClientFactory
	{

		private ILoadBalancer GrpcLoadBalance { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private CircuitBreakerPolicy CircuitBreakerPolicy { get; }

		private CircuitBreakerServiceBuilder CircuitBreakerServiceBuilder { get; }


		public ClientFactory(ILoadBalancer grpcLoadBalance, GrpcClientConfiguration grpcClientConfiguration, CircuitBreakerPolicy circuitBreakerPolicy, CircuitBreakerServiceBuilder circuitBreakerServiceBuilder)
		{
			GrpcLoadBalance = grpcLoadBalance;
			GrpcClientConfiguration = grpcClientConfiguration;
			CircuitBreakerPolicy = circuitBreakerPolicy;
			CircuitBreakerServiceBuilder = circuitBreakerServiceBuilder;
		}


		public TService Get<TService>(string serviceName) where TService : IService<TService>
		{
			var channel = GrpcLoadBalance.GetNextChannel(serviceName);
			var caller = channel.Intercept(new CircuitBreakerInterceptor(CircuitBreakerPolicy, CircuitBreakerServiceBuilder, typeof(TService), GrpcClientConfiguration.CircuitBreakerOption));
			return MagicOnionClient.Create<TService>(caller);
		}
	}
}
