using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public class DependencyInjectionInterceptor : Interceptor
	{
		private IServiceProvider ServiceProvider { get; }

		public DependencyInjectionInterceptor(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}



		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
		{
			using (var serviceScope = ServiceProvider.CreateScope())
			{
				context.RequestHeaders.Add(serviceScope.ServiceProvider.GetService<ServiceProviderMetadataEntry>());
				return await base.UnaryServerHandler(request, context, continuation);
			}
		}
	}
}
