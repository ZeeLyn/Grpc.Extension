using MagicOnion;

namespace Grpc.Extension.Client
{
	public interface IClientFactory
	{
		TService GetClient<TService>(string serviceName) where TService : IService<TService>;

		TStreamingHub GetStreamingHubClient<TStreamingHub, TReceiver>(string serviceName, TReceiver receiver)
			where TStreamingHub : IStreamingHub<TStreamingHub, TReceiver>;
	}
}
