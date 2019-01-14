using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MagicOnion;

namespace Grpc.ServiceInterface
{
	public interface IChatReceiver
	{
		void OnReceiver(string message);
	}

	public interface IChatHub : IStreamingHub<IChatHub, IChatReceiver>
	{
		Task<string> ReceiverAsync(string message);
	}
}
