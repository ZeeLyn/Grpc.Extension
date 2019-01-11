using System.Collections.Generic;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class InjectionCommand
	{
		public string Command { get; set; }

		public string[] Namespace { get; set; }
	}
}
