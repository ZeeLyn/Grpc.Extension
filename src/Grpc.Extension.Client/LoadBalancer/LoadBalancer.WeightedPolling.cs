using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalancer
{
	public class LoadBalancerWeightedPolling : ILoadBalancer
	{
		internal ChannelFactory ChannelFactory { get; }

		public LoadBalancerWeightedPolling(ChannelFactory channelFactory)
		{
			ChannelFactory = channelFactory;
		}

		public override Channel GetNextChannel(string serviceName)
		{
			var nodes = ChannelFactory.GetChannelNodes(serviceName);
			int index = -1;
			int total = 0;
			for (var i = 0; i < nodes.Count; i++)
			{
				nodes[i].CurrentWeight += nodes[i].Weight;
				total += nodes[i].Weight;
				if (index == -1 || nodes[index].CurrentWeight < nodes[i].CurrentWeight)
				{
					index = i;
				}
			}

			nodes[index].CurrentWeight -= total;
			return nodes[index].Channel;
		}
	}
}
