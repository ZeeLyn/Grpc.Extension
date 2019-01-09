using System;
using MagicOnion;

namespace Grpc.ServiceInterface
{
	public interface IHelloService : IService<IHelloService>
	{
		UnaryResult<string> Say(string name);
	}
}
