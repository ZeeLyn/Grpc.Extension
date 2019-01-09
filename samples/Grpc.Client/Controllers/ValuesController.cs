using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Client;
using Grpc.ServiceInterface;
using Microsoft.AspNetCore.Mvc;

namespace Grpc.Client.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{

		private ClientFactory ClientFactory { get; }

		private GrpcService GrpcService { get; }

		public ValuesController(ClientFactory clientFactory, GrpcService grpcService)
		{
			ClientFactory = clientFactory;
			GrpcService = grpcService;
		}

		// GET api/values
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			//var ch = new Channel("192.168.1.129", 50054, ChannelCredentials.Insecure);
			//var client = new Hello.HelloClient(ch);
			//GrpcService.Client<Hello.HelloClient, HelloReply>("", c => { return c.SayHello(new HelloRequest()); });
			//ClientFactory.Get<Hello.HelloClient>("grpc-server").Invok();
			var client = ClientFactory.Get<IHelloService>("grpc-server");
			return Ok(await client.Say("owen"));
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
