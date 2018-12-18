using System.Linq;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalance
{
	public class GrpcLoadBalanceRound : GrpcLoadBalance
	{
		protected ChannelFactory ChannelFactory { get; }

		private int _index = -1;

		public GrpcLoadBalanceRound(ChannelFactory channelFactory)
		{
			ChannelFactory = channelFactory;
		}

		private static readonly object LockObject = new object();

		public override Channel GetService(string serviceName)
		{
			lock (LockObject)
			{

				var channels = ChannelFactory.GetChannels(serviceName);
				if (!channels.Any())
					throw new System.Exception("No service node");
				_index++;
				if (_index > channels.Count - 1)
					_index = 0;
				return channels[_index];
			}
		}
	}
}
