using System;
using System.Threading.Tasks;
using Grpc.Extension.Core;
using MagicOnion;

namespace Grpc.ServiceInterface
{
	public interface IHelloService : IService<IHelloService>
	{
		[NonCircuitBreaker]
		UnaryResult<string> Say(string name);
	}
}
