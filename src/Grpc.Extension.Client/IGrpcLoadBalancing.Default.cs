using System.Linq;
using Grpc.Core;

namespace Grpc.Extension.Client
{
	public class GrpcLoadBalancing : IGrpcLoadBalancing
	{
		protected ChannelFactory ChannelFactory { get; }

		public GrpcLoadBalancing(ChannelFactory channelFactory)
		{
			ChannelFactory = channelFactory;
		}

		public virtual Channel GetService(string serviceName)
		{
			var c = ChannelFactory.GetChannels(serviceName);
			return c.FirstOrDefault();
		}
	}
}
