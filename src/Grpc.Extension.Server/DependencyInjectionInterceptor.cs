using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Extension.Server
{
	public class DependencyInjectionInterceptor : Interceptor
	{
		private IServiceProvider ServiceProvider { get; }

		private PropertyInfo[] ServiceProperties { get; }

		private object ServiceInstance { get; }

		public DependencyInjectionInterceptor(IServiceProvider serviceProvider, object serviceInstance)
		{
			ServiceProvider = serviceProvider;
			ServiceInstance = serviceInstance;
			var serviceType = serviceInstance.GetType();
			ServiceProperties = serviceType.GetProperties();
		}



		public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
		{
			using (var serviceScope = ServiceProvider.CreateScope())
			{
				foreach (var p in ServiceProperties)
				{
					var value = serviceScope.ServiceProvider.GetRequiredService(p.PropertyType);
					p.SetValue(ServiceInstance, value);
				}

				//context.RequestHeaders.Add(serviceScope.ServiceProvider.GetService<ServiceProviderMetadataEntry>());
				return await base.UnaryServerHandler(request, context, continuation);
			}
		}
	}
}
