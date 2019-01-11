using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public class InjectionCommand
	{
		public string Command { get; set; }

		public IEnumerable<string> Namespace { get; set; }
	}
}
