using Grpc.Extension.Core;
using MagicOnion;

namespace Grpc.ServiceInterface
{
	public interface IHelloService : IService<IHelloService>
	{
		[CircuitBreaker(InjectionNamespace = new[] { "" }, FallbackInjectionScript = "return \"失败1\";")]
		UnaryResult<string> Say(string name);
	}
}
