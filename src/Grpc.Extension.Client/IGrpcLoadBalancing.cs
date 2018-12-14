using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extension.Client
{
	public interface IGrpcLoadBalancing
	{
		ServiceEndPoint GetService(string serviceName);
	}
}
