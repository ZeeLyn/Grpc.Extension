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
			var client = ClientFactory.GetClient<IHelloService>("grpc-server");
			var r = await client.Say("owen");

			//var hub = ClientFactory.GetStreamingHubClient<IChatHub, IChatReceiver>("grpc-server", new HubRecevier());
			//var rec = await hub.ReceiverAsync("你好");
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
