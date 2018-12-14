using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extension.Client
{
	public class GrpcLoadBalancing : IGrpcLoadBalancing
	{
		public ServiceEndPoint GetService(string serviceName)
		{
			throw new NotImplementedException();
		}
	}
}
