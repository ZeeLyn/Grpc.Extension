using System;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Client
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddGrpcClient(this IServiceCollection serviceCollection)
		{
			serviceCollection.AddSingleton<ChannelFactory>();
			return serviceCollection;
		}
	}
}
