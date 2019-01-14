using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.ServiceInterface;

namespace Grpc.Client
{
	public class HubRecevier : IChatReceiver
	{
		public void OnReceiver(string message)
		{
			var m = message;
		}

	}
}
