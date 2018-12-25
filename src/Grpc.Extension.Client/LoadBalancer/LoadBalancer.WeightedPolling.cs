using System.Linq;
using Grpc.Core;

namespace Grpc.Extension.Client.LoadBalancer
{
	public class LoadBalancerWeightedPolling : ILoadBalancer
	{
		internal ChannelFactory ChannelFactory { get; }

		private static readonly object LockObject = new object();

		public LoadBalancerWeightedPolling(ChannelFactory channelFactory)
		{
			ChannelFactory = channelFactory;
		}

		public override Channel GetNextChannel(string serviceName)
		{
			lock (LockObject)
			{
				var nodes = ChannelFactory.GetChannelNodes(serviceName);
				if (!nodes.Any())
					throw new System.Exception($"Service {serviceName} did not find available nodes.");
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
}
