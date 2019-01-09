using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Server;
using Grpc.ServiceInterface;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.Extensions.Configuration;

namespace Grpc.Server.WebApp
{
	public class HelloService : ServiceBase<IHelloService>, IHelloService
	{
		//private GrpcServerConfiguration GrpcServerConfiguration { get; }

		//public HelloService(GrpcServerConfiguration grpcServerConfiguration)
		//{
		//	GrpcServerConfiguration = grpcServerConfiguration;
		//}

		public async UnaryResult<string> Say(string name)
		{

			return "hello " + name;
		}
	}
}
