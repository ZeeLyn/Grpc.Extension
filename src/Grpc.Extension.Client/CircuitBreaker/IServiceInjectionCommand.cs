using System;
using System.Threading.Tasks;

namespace Grpc.Extension.Client.CircuitBreaker
{
	public interface IServiceInjectionCommand
	{
		InjectionCommand GetCommand(Type serviceType, string serviceName);

		Task<object> Run(string command, params string[] injectionNamespaces);
	}
}
