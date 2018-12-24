using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Server;
using Microsoft.Extensions.Configuration;

namespace Grpc.Server.WebApp
{
	public class HelloService : Hello.HelloBase, IGrpcService
	{
		public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			var rs = context.GetService<RepertoryService>();
			var conf = context.GetService<IConfiguration>();
			return await Task.FromResult(new HelloReply { Message = $"Port:{conf.GetSection("GrpcServer:Port").Get<int>()}:{rs.GetGuid()} Hello " + request.Name });
		}
	}
}
