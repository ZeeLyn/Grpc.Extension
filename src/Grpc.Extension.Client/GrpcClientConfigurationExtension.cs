﻿using System;
using System.Linq;
using Consul;
using Grpc.Core;
using Grpc.Extension.Client.LoadBalance;

namespace Grpc.Extension.Client
{
	public static class GrpcClientConfigurationExtension
	{
		public static GrpcClientConfiguration AddLoadBalance<TGrpcLoadBalance>(this GrpcClientConfiguration gRpcClientConfiguration) where TGrpcLoadBalance : GrpcLoadBalance
		{
			gRpcClientConfiguration.GrpcLoadBalance = typeof(TGrpcLoadBalance);
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddLoadBalance(this GrpcClientConfiguration gRpcClientConfiguration, Type type)
		{
			if (!typeof(GrpcLoadBalance).IsAssignableFrom(type))
				throw new TypeUnloadedException($"Type {type} does not implement GrpcLoadBalance");
			gRpcClientConfiguration.GrpcLoadBalance = type;
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddConsul(this GrpcClientConfiguration gRpcClientConfiguration, Action<ConsulClientConfiguration> action)
		{
			gRpcClientConfiguration.ConsulClientConfiguration = new ConsulClientConfiguration();
			action?.Invoke(gRpcClientConfiguration.ConsulClientConfiguration);
			return gRpcClientConfiguration;
		}


		public static GrpcClientConfiguration AddServiceCredentials(this GrpcClientConfiguration gRpcClientConfiguration, string serviceName, ChannelCredentials channelCredentials)
		{
			gRpcClientConfiguration.ServicesCredentials[serviceName] = channelCredentials;
			return gRpcClientConfiguration;
		}


		public static GrpcClientConfiguration AddClient(this GrpcClientConfiguration gRpcClientConfiguration,
			params Type[] types)
		{
			if (types.Any(p => !typeof(ClientBase).IsAssignableFrom(p)))
				throw new InvalidOperationException("The added type is not a grpc client implementation.");
			gRpcClientConfiguration.ClientTypes.AddRange(types);
			return gRpcClientConfiguration;
		}

		public static GrpcClientConfiguration AddClient<TClient>(this GrpcClientConfiguration gRpcClientConfiguration) where TClient : ClientBase
		{
			gRpcClientConfiguration.ClientTypes.Add(typeof(TClient));
			return gRpcClientConfiguration;
		}
	}
}
