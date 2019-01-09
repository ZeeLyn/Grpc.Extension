using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extension.Server.ServiceDiscovery
{
	public interface IServiceDiscovery
	{
		Task<bool> RegisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, int? weight = default, CancellationToken cancellationToken = default);

		Task<bool> DeregisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, CancellationToken cancellationToken = default);
	}
}
