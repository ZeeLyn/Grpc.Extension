using System;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalancer;
using Grpc.Extension.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Client
{
	public static class GrpcClientConfigurationExtension
	{
		public static GrpcClientConfiguration AddLoadBalancer<TGrpcLoadBalance>(this GrpcClientConfiguration gRpcClientConfiguration) where TGrpcLoadBalance : ILoadBalancer
		{
			gRpcClientConfiguration.GrpcLoadBalance = typeof(TGrpcLoadBalance);
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddLoadBalancer(this GrpcClientConfiguration gRpcClientConfiguration, Type type)
		{
			if (!typeof(ILoadBalancer).IsAssignableFrom(type))
				throw new TypeUnloadedException($"Type {type} does not implement ILoadBalancer");
			gRpcClientConfiguration.GrpcLoadBalance = type;
			return gRpcClientConfiguration;
		}


		public static GrpcClientConfiguration AddServiceDiscovery<TDiscovery>(
			this GrpcClientConfiguration gRpcClientConfiguration, IClientConfiguration clientConfiguration) where TDiscovery : IServiceDiscovery
		{
			gRpcClientConfiguration.ConsulClientConfiguration = clientConfiguration;
			gRpcClientConfiguration.ServiceCollection.AddSingleton(typeof(IServiceDiscovery), typeof(TDiscovery));
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddConsulServiceDiscovery(this GrpcClientConfiguration gRpcClientConfiguration, Action<ConsulConfiguration> action)
		{
			var client = new ConsulConfiguration();
			action?.Invoke(client);
			return gRpcClientConfiguration.AddServiceDiscovery<ConsulServiceDiscovery>(client);
		}



		public static GrpcClientConfiguration AddServiceCredentials(this GrpcClientConfiguration gRpcClientConfiguration, string serviceName, ChannelCredentials channelCredentials)
		{
			if (string.IsNullOrWhiteSpace(serviceName))
				throw new ArgumentNullException(nameof(serviceName));
			gRpcClientConfiguration.ServicesCredentials[serviceName] = channelCredentials;
			return gRpcClientConfiguration;
		}


		public static GrpcClientConfiguration AddCircuitBreaker(this GrpcClientConfiguration gRpcClientConfiguration, Action<CircuitBreakerOption> action)
		{
			var option = new CircuitBreakerOption();
			action?.Invoke(option);
			gRpcClientConfiguration.CircuitBreakerOption = option;
			return gRpcClientConfiguration;
		}
	}
}
