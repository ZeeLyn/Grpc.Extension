using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Core
{
	public class ConsulServiceDiscovery : IServiceDiscovery
	{
		private ILogger Logger { get; }
		public ConsulServiceDiscovery(ILogger<ConsulServiceDiscovery> logger)
		{
			Logger = logger;
		}

		public async Task<bool> RegisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, ServerPort serverPort, int? weight = default,
			CancellationToken cancellationToken = default)
		{
			if (!(clientConfig is ConsulConfiguration client))
				throw new ArgumentNullException(nameof(clientConfig));
			if (!(serviceConfig is ConsulServiceConfiguration service))
				throw new ArgumentNullException(nameof(serviceConfig));

			using (var consul = new ConsulClient(conf =>
			{
				conf.Address = client.Address;
				conf.Datacenter = client.Datacenter;
				conf.Token = client.Token;
				conf.WaitTime = client.WaitTime;
			}))
			{
				if (weight.HasValue)
				{
					if (service.Meta == null)
						service.Meta = new Dictionary<string, string>();
					service.Meta.Add("X-Weight", weight.ToString());
				}

				//Register service to consul agent 
				var result = await consul.Agent.ServiceRegister(new AgentServiceRegistration
				{
					Address = serverPort.Host,
					Port = serverPort.Port,
					ID = string.IsNullOrWhiteSpace(service.ServiceId) ? $"{serverPort.Host}:{serverPort.Port}" : service.ServiceId,
					Name = string.IsNullOrWhiteSpace(service.ServiceName) ? $"{serverPort.Host}:{serverPort.Port}" : service.ServiceName,
					EnableTagOverride = service.EnableTagOverride,
					Meta = service.Meta,
					Tags = service.Tags,
					Check = new AgentServiceCheck
					{
						DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(20),
						GRPC = $"{serverPort.Host}:{serverPort.Port}",
						Timeout = TimeSpan.FromSeconds(3),
						Interval = service.HealthCheckInterval
					}
				}, cancellationToken);
				if (result.StatusCode != HttpStatusCode.OK)
				{
					Logger.LogError("--------------->  Registration service failed:{0}", result.StatusCode);
					throw new ConsulRequestException("Registration service failed.", result.StatusCode);
				}
				Logger.LogInformation("---------------> Consul service registration completed");
				return result.StatusCode != HttpStatusCode.OK;
			}
		}

		public async Task<bool> DeregisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, ServerPort serverPort, CancellationToken cancellationToken = default)
		{
			if (!(clientConfig is ConsulConfiguration client))
				throw new ArgumentNullException(nameof(clientConfig));
			if (!(serviceConfig is ConsulServiceConfiguration service))
				throw new ArgumentNullException(nameof(serviceConfig));
			using (var consul = new ConsulClient(conf =>
			{
				conf.Address = client.Address;
				conf.Datacenter = client.Datacenter;
				conf.Token = client.Token;
				conf.WaitTime = client.WaitTime;
			}))
			{

				var serviceId = string.IsNullOrWhiteSpace(service.ServiceId)
					? $"{serverPort.Host}:{serverPort.Port}"
					: service.ServiceId;
				var result = await consul.Agent.ServiceDeregister(serviceId, cancellationToken);
				if (result.StatusCode != HttpStatusCode.OK)
				{
					Logger.LogError("--------------->  Deregistration service failed:{0}", result.StatusCode);
					throw new ConsulRequestException("Deregistration service failed.", result.StatusCode);
				}

				return result.StatusCode == HttpStatusCode.OK;
			}
		}

		public async Task<List<ServiceEntry>> GetServices(IClientConfiguration clientConfig, string serviceName, string tag = "", bool passingOnly = true,
			CancellationToken cancellationToken = default)
		{
			if (!(clientConfig is ConsulConfiguration client))
				throw new ArgumentNullException(nameof(clientConfig));

			using (var consul = new ConsulClient(conf =>
			{
				conf.Address = client.Address;
				conf.Datacenter = client.Datacenter;
				conf.Token = client.Token;
				conf.WaitTime = client.WaitTime;
			}))
			{
				try
				{
					var result = await consul.Health.Service(serviceName, tag, passingOnly, cancellationToken);
					if (result.StatusCode != System.Net.HttpStatusCode.OK)
					{
						Logger.LogError("Get the health service error:{0}", result.StatusCode);
					}

					return result.Response.ToList();
				}
				catch (Exception ex)
				{
					Logger.LogError("Get the health service error:{0}\n{1}", ex.Message, ex.StackTrace);
					throw;
				}
			}
		}
	}
}
