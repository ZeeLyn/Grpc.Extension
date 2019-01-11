using Microsoft.AspNetCore.Builder;

namespace Grpc.Extension.Core
{
	public interface IServerBootstrap
	{
		void Start(IApplicationBuilder app);

		void OnStopping();

		void OnStopped();
	}
}
