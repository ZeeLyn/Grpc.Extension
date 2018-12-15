using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;

namespace Grpc.Extension.Client
{
	public interface IGrpcLoadBalancing
	{
		Channel GetService(string serviceName);
	}
}
