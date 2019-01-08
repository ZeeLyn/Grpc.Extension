using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Logging;


namespace Grpc.Extension.Server.ServiceDiscovery
{
	public class ConsulServiceDiscovery : IServiceDiscovery
	{
		private ILogger Logger { get; }
		public ConsulServiceDiscovery(ILogger<ServerBootstrap> logger)
		{
			Logger = logger;
		}

		public async Task<bool> RegisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, int? weight = default,
			CancellationToken cancellationToken = default)
		{
			var client = clientConfig as ConsulConfiguration;
			var service = serviceConfig as ConsulServiceConfiguration;
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
					Address = service.Address,
					Port = service.Port,
					ID = string.IsNullOrWhiteSpace(service.ServiceId) ? $"{service.Address}:{service.Port}" : service.ServiceId,
					Name = string.IsNullOrWhiteSpace(service.ServiceName) ? $"{service.Address}:{service.Port}" : service.ServiceName,
					EnableTagOverride = service.EnableTagOverride,
					Meta = service.Meta,
					Tags = service.Tags
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

		public async Task<bool> DeregisterAsync(IClientConfiguration clientConfig, IServiceConfiguration serviceConfig, CancellationToken cancellationToken = default)
		{
			var client = clientConfig as ConsulConfiguration;
			var service = serviceConfig as ConsulServiceConfiguration;
			using (var consul = new ConsulClient(conf =>
			{
				conf.Address = client.Address;
				conf.Datacenter = client.Datacenter;
				conf.Token = client.Token;
				conf.WaitTime = client.WaitTime;
			}))
			{
				var result = await consul.Agent.ServiceDeregister(string.IsNullOrWhiteSpace(service.ServiceId) ? $"{service.Address}:{service.Port}" : service.ServiceId, cancellationToken);
				if (result.StatusCode != HttpStatusCode.OK)
				{
					Logger.LogError("--------------->  Deregistration service failed:{0}", result.StatusCode);
					throw new ConsulRequestException("Deregistration service failed.", result.StatusCode);
				}

				return result.StatusCode == HttpStatusCode.OK;
			}
		}
	}
}
