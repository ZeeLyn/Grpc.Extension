using System;

namespace Grpc.Extension.Core
{
	[AttributeUsage(AttributeTargets.Method)]
	public class NonCircuitBreakerAttribute : Attribute
	{
	}
}
