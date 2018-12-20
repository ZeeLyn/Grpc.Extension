using System.Threading;
using System.Threading.Tasks;
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

			var channelFactory = app.ApplicationServices.GetService<ChannelFactory>();

			applicationLifetime.ApplicationStopping.Register(() =>
			{
				CancellationTokenSource.Cancel();

			});

			Task.Factory.StartNew(async () =>
			{
				while (!CancellationTokenSource.Token.IsCancellationRequested)
				{
					await channelFactory.RefreshChannels(CancellationTokenSource.Token);
					await Task.Delay(configure.ChannelStatusCheckInterval, CancellationTokenSource.Token);
				}
			}, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);



			return app;
		}
	}
}
