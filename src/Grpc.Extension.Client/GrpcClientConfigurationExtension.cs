using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extension.Client
{
	public static class GrpcClientConfigurationExtension
	{
		public static GrpcClientConfiguration AddLoadBalancing<T>(this GrpcClientConfiguration gRpcClientConfiguration) where T : IGrpcLoadBalancing
		{
			gRpcClientConfiguration.GrpcLoadBalancing = typeof(T);
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddLoadBalancing(this GrpcClientConfiguration gRpcClientConfiguration, Type type)
		{
			gRpcClientConfiguration.GrpcLoadBalancing = type;
			return gRpcClientConfiguration;
		}
	}
}
