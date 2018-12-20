using System;
using Microsoft.Extensions.Logging;


namespace Grpc.Server.WebApp
{
	public class RepertoryService : IDisposable
	{
		private ILogger Logger { get; }

		public RepertoryService(ILogger<RepertoryService> logger)
		{
			Logger = logger;
		}

		public string GetGuid()
		{
			return Guid.NewGuid().ToString();
		}

		public void Dispose()
		{
			Logger.LogInformation("------------------Dispose---------------");
		}
	}
}
