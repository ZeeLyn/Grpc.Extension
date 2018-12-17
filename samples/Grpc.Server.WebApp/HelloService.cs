using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace Grpc.Server.WebApp
{
	public class HelloService : Hello.HelloBase
	{
		private IConfiguration configuration;

		public HelloService()
		{
		}

		public HelloService(IConfiguration config)
		{
			configuration = config;
		}

		public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			return Task.FromResult(new HelloReply { Message = $"{configuration == null} Hello " + request.Name });
		}
	}
}
