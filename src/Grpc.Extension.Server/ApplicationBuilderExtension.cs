using System.Threading;
using Grpc.Extension.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public static class ApplicationBuilderExtension
	{
		private static CancellationTokenSource CancellationTokenSource { get; }

		static ApplicationBuilderExtension()
		{
			CancellationTokenSource = new CancellationTokenSource();
		}


		public static IApplicationBuilder UseGrpcServer(this IApplicationBuilder app)
		{
			app.ApplicationServices.GetService<IServerBootstrap>().Start(app);
			return app;
		}
	}
}
