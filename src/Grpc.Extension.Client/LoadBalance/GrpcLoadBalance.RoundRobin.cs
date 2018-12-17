using System.Linq;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalance
{
	public class GrpcLoadBalanceRoundRobin : GrpcLoadBalance
	{
		protected ChannelFactory ChannelFactory { get; }

		private int _index = -1;

		public GrpcLoadBalanceRoundRobin(ChannelFactory channelFactory)
		{
			ChannelFactory = channelFactory;
		}

		private static readonly object LockObject = new object();

		public override Channel GetService(string serviceName)
		{
			lock (LockObject)
			{
				_index++;
				var channels = ChannelFactory.GetChannels(serviceName);

				if (_index > channels.Count - 1)
					_index = 0;

				return channels[_index];
			}
		}
	}
}
