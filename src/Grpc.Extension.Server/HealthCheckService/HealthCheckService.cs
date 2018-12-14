using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Health.V1;

namespace Grpc.Extension.Server.HealthCheckService
{
	public class HealthCheckService : Health.V1.Health.HealthBase
	{
		public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
		{
			return await Task.FromResult(new HealthCheckResponse
			{
				Status = HealthCheckResponse.Types.ServingStatus.Serving
			});
		}
	}
}
