using System;
using System.Threading.Tasks;
using Grpc.Extension.Client;
using MagicOnion;

namespace Grpc.ServiceInterface
{
	public interface IHelloService : IService<IHelloService>
	{
		[CircuitBreaker]
		UnaryResult<string> Say(string name);
	}
}
