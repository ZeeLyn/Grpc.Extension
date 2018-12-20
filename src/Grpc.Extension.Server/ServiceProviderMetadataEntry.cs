using System;
using Grpc.Core;

namespace Grpc.Extension.Server
{
	public class ServiceProviderMetadataEntry : Metadata.Entry
	{
		public const string MetadataKey = "X-ServiceProvider";
		public IServiceProvider ServiceProvider { get; }

		public ServiceProviderMetadataEntry(IServiceProvider serviceProvider) : base(MetadataKey, string.Empty)
		{
			ServiceProvider = serviceProvider;
		}

		public ServiceProviderMetadataEntry(string key, byte[] valueBytes) : base(key, valueBytes)
		{
		}

		public ServiceProviderMetadataEntry(string key, string value) : base(key, value)
		{
		}
	}
}
