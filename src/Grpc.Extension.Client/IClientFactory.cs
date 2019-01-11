using System;
using System.Collections.Generic;
using System.Text;
using MagicOnion;

namespace Grpc.Extension.Client
{
	public interface IClientFactory
	{
		TService Get<TService>(string serviceName) where TService : IService<TService>;
	}
}
