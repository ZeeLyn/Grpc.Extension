using System;

namespace Grpc.Extension.Core
{
	public class CircuitBreakerAttribute : Attribute
	{
		public string FallbackInjection { get; set; }

		public string[] InjectionNamespace { get; set; }
	}
}
