using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.ServiceInterface;
using MagicOnion.Server.Hubs;

namespace Grpc.Server.WebApp
{
	public class ChatService : StreamingHubBase<IChatHub, IChatReceiver>, IChatHub
	{
		public async Task<string> ReceiverAsync(string message)
		{

			return await Task.FromResult($"Receiver：{message}");

		}
	}
}
