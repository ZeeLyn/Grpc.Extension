using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extension.Client
{
	public interface IChannelFactory
	{
		List<ChannelNode> GetChannelNodes(string serviceName, ChannelNodeStatus status = ChannelNodeStatus.Passing);

		Task RefreshChannels(CancellationToken cancellationToken);
	}
}
