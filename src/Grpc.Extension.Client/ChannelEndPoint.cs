using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;

namespace Grpc.Extension.Client
{
	public class ChannelEndPoint
	{
		public string Address { get; set; }

		public int Port { get; set; }

		public ChannelEndPointStatus Status { get; set; } = ChannelEndPointStatus.Critical;

		public Channel Channel { get; set; }
	}

	public enum ChannelEndPointStatus
	{
		Critical = 0,
		Passing = 1
	}
}
