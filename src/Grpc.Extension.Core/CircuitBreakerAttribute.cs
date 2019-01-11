using System;

namespace Grpc.Extension.Core
{
	public class CircuitBreakerAttribute : Attribute
	{
		public string FallbackInjectionScript { get; set; }

		public string[] InjectionNamespace { get; set; }
	}
}
