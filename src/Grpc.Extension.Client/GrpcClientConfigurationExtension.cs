using System;
using Consul;
using Grpc.Extension.Client.LoadBalance;

namespace Grpc.Extension.Client
{
	public static class GrpcClientConfigurationExtension
	{
		public static GrpcClientConfiguration AddLoadBalance<T>(this GrpcClientConfiguration gRpcClientConfiguration) where T : GrpcLoadBalance
		{
			gRpcClientConfiguration.GrpcLoadBalance = typeof(T);
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddLoadBalance(this GrpcClientConfiguration gRpcClientConfiguration, Type type)
		{
			if (typeof(GrpcLoadBalance).IsAssignableFrom(type))
				throw new TypeUnloadedException($"Type {type} does not implement interface IGrpcLoadBalancing");
			gRpcClientConfiguration.GrpcLoadBalance = type;
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddConsul(this GrpcClientConfiguration gRpcClientConfiguration, Action<ConsulClientConfiguration> action, params ServiceConfiguration[] serviceConfigurations)
		{
			gRpcClientConfiguration.ConsulClientConfiguration = new ConsulClientConfiguration();
			action?.Invoke(gRpcClientConfiguration.ConsulClientConfiguration);
			gRpcClientConfiguration.ServicesConfiguration.AddRange(serviceConfigurations);
			return gRpcClientConfiguration;
		}
	}
}
