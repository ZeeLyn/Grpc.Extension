using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Server;
using Microsoft.Extensions.Configuration;

namespace Grpc.Server.WebApp
{
	public class HelloService : Hello.HelloBase, IGrpcService
	{
		private IConfiguration configuration;

		public RepertoryService RepertoryService { get; set; }

		public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			return await Task.FromResult(new HelloReply { Message = $"1:{RepertoryService.GetGuid()} Hello " + request.Name });
		}
	}
}
