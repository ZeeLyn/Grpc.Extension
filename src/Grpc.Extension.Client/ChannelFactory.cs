using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Grpc.Core;

namespace Grpc.Extension.Client
{
	public class ChannelFactory
	{
		private Dictionary<string, List<ChannelEndPoint>> ServiceEndPoints { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private object LockObject { get; set; }
		private static AsyncLock asyncLock = new AsyncLock();

		public ChannelFactory(GrpcClientConfiguration _grpcClientConfiguration)
		{
			ServiceEndPoints = new Dictionary<string, List<ChannelEndPoint>>();
			GrpcClientConfiguration = _grpcClientConfiguration;
		}



		public async Task RefreshChannels()
		{
			using (asyncLock.GetLockAsync(1000 * 10))
			{
				foreach (var serviceName in GrpcClientConfiguration.GrpcServiceName)
				{
					var entries = await GetServiceEndPoints(serviceName);
					if (!ServiceEndPoints.ContainsKey(serviceName))
					{
						ServiceEndPoints.Add(serviceName, entries.Select(p => new ChannelEndPoint
						{
							Address = p.Service.Address,
							Port = p.Service.Port,
							Channel = new Channel(p.Service.Address, p.Service.Port, ChannelCredentials.Insecure),
							Status = ChannelEndPointStatus.Passing
						}).ToList());
						continue;
					}

					if (ServiceEndPoints.TryGetValue(serviceName, out var endPoints))
					{
						var newEndPoints = entries.FindAll(p => !endPoints.Any(e => e.Address == p.Service.Address && e.Port == p.Service.Port));
						if (newEndPoints.Any())
						{
							endPoints.AddRange(newEndPoints.Select(p => new ChannelEndPoint
							{
								Address = p.Service.Address,
								Port = p.Service.Port,
								Channel = new Channel(p.Service.Address, p.Service.Port, ChannelCredentials.Insecure),
								Status = ChannelEndPointStatus.Passing
							}));
						}

						foreach (var entry in newEndPoints)
						{
							var endPoint = endPoints.FirstOrDefault(p => p.Address == entry.Service.Address && p.Port == entry.Service.Port);
							if (endPoint == null)
								endPoints.Add(new ChannelEndPoint
								{
									Address = entry.Service.Address,
									Port = entry.Service.Port,
									Channel = new Channel(entry.Service.Address, entry.Service.Port, ChannelCredentials.Insecure),
									Status = ChannelEndPointStatus.Passing
								});
							continue;

						}
					}
				}
			}
		}


		public async Task<List<ServiceEntry>> GetServiceEndPoints(string serviceId, string tag = "", bool passingOnly = true)
		{
			using (var consul = new ConsulClient(config => { config.Address = new Uri("http://192.168.1.142:8500"); }))
			{
				var result = await consul.Health.Service(serviceId, tag, passingOnly);
				return result.Response.ToList();
			}
		}
	}
}
