using System;
using System.Linq;
using Grpc.Core;

namespace Grpc.Extension.Server
{
	public static class CommonExtensionMethods
	{
		public static IServiceProvider GetServiceProvider(this ServerCallContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (!(context.RequestHeaders.FirstOrDefault(p => p.Key.Equals(ServiceProviderMetadataEntry.MetadataKey, StringComparison.CurrentCultureIgnoreCase)) is ServiceProviderMetadataEntry serviceProvider))
				throw new InvalidOperationException($"{ServiceProviderMetadataEntry.MetadataKey} request header metadata not found.");

			return serviceProvider.ServiceProvider;
		}
	}
}
