using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Server.HealthCheckService
{
	public class HealthCheckService : Health.V1.Health.HealthBase
	{
		private ILogger Logger { get; }

		public HealthCheckService(ILogger<HealthCheckService> logger)
		{
			Logger = logger;
		}

		public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
		{
			Logger.LogInformation("---------------> Health checking...");
			return await Task.FromResult(new HealthCheckResponse
			{
				Status = HealthCheckResponse.Types.ServingStatus.Serving
			});
		}
	}
}
