using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Client;
using Grpc.Extension.Client.CircuitBreaker;
using Grpc.ServiceInterface;
using MagicOnion;
using Microsoft.AspNetCore.Mvc;

namespace Grpc.Client.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{

		private IClientFactory ClientFactory { get; }

		private IServiceInjectionCommand ServiceInjectionCommand { get; }

		public ValuesController(IClientFactory clientFactory, IServiceInjectionCommand serviceInjectionCommand)
		{
			ClientFactory = clientFactory;
			ServiceInjectionCommand = serviceInjectionCommand;
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
			var r = await client.Say("owen");
			return Ok(new
			{
				Result = r,
				Command = await ServiceInjectionCommand.Run("return new{result=1};")
			});
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
