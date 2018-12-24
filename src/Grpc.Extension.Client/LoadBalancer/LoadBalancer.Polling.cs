using System.Linq;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalancer
{
	public class LoadBalancerPolling : ILoadBalancer
	{
		internal ChannelFactory ChannelFactory { get; }

		private int _index = -1;

		public LoadBalancerPolling(ChannelFactory channelFactory)
		{
			ChannelFactory = channelFactory;
		}

		private static readonly object LockObject = new object();

		public override Channel GetChannel(string serviceName)
		{
			lock (LockObject)
			{
				var channels = ChannelFactory.GetChannels(serviceName);
				if (!channels.Any())
					throw new System.Exception($"Service {serviceName} did not find available nodes.");
				_index++;
				if (_index > channels.Count - 1)
					_index = 0;
				return channels[_index];
			}
		}
	}
}
