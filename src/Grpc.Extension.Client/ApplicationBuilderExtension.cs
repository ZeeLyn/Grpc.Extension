using System.Threading;
using System.Threading.Tasks;
using Grpc.Extension.Client.CircuitBreaker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Client
{
	public static class ApplicationBuilderExtension
	{
		private static CancellationTokenSource CancellationTokenSource { get; }


		static ApplicationBuilderExtension()
		{
			CancellationTokenSource = new CancellationTokenSource();
		}


		public static IApplicationBuilder UseGrpcClient(this IApplicationBuilder app)
		{
			var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();

			var configure = app.ApplicationServices.GetService<GrpcClientConfiguration>();

			var channelFactory = app.ApplicationServices.GetService<IChannelFactory>();

			var serviceBreakerBuilder = app.ApplicationServices.GetService<CircuitBreakerServiceBuilder>();

			applicationLifetime.ApplicationStopping.Register(() =>
			{
				CancellationTokenSource.Cancel();
			});

			serviceBreakerBuilder.InitializeService();

			Task.Factory.StartNew(async () =>
			{
				while (!CancellationTokenSource.Token.IsCancellationRequested)
				{
					await channelFactory.RefreshChannelsStatus(CancellationTokenSource.Token);
					await Task.Delay(configure.ChannelStatusCheckInterval, CancellationTokenSource.Token);
				}

			}, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

			return app;
		}
	}
}
