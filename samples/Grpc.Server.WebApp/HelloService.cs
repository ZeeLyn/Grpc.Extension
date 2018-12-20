using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Server;

namespace Grpc.Server.WebApp
{
	public class HelloService : Hello.HelloBase, IGrpcService
	{
		public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
		{
			var rs = context.GetService<RepertoryService>();
			return await Task.FromResult(new HelloReply { Message = $"1:{rs.GetGuid()} Hello " + request.Name });
		}
	}
}
