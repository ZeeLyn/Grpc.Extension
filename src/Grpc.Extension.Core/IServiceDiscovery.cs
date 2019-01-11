using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Grpc.Core;

namespace Grpc.Extension.Core
{
	public interface IServiceDiscovery
	{
		Task<bool> RegisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, ServerPort serverPort, int? weight = default, CancellationToken cancellationToken = default);

		Task<bool> DeregisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, ServerPort serverPort, CancellationToken cancellationToken = default);

		Task<List<ServiceEntry>> GetServices(IClientConfiguration clientConfig, string serviceName, string tag = "", bool passingOnly = true,
			CancellationToken cancellationToken = default);
	}
}
