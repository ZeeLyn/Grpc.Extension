using Grpc.Core;

namespace Grpc.Extension.Client
{
	public class ChannelNode
	{
		public string Address { get; set; }

		public int Port { get; set; }

		public ChannelNodeStatus Status { get; set; } = ChannelNodeStatus.Critical;

		public int Weight { get; set; }
		protected internal int CurrentWeight { get; set; }

		public Channel Channel { get; set; }


	}

	public enum ChannelNodeStatus
	{
		Critical = 0,
		Passing = 1
	}
}
