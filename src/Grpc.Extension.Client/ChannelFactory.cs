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
		private ConcurrentDictionary<string, List<ChannelEndPoint>> EndPoint { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private object LockObject { get; set; }

		public ChannelFactory(GrpcClientConfiguration _grpcClientConfiguration)
		{
			EndPoint = new ConcurrentDictionary<string, List<ChannelEndPoint>>();
			GrpcClientConfiguration = _grpcClientConfiguration;
		}

		public async Task RefurbishChannel()
		{

			lock (LockObject)
			{
				foreach (var serviceName in GrpcClientConfiguration.GrpcServiceName)
				{
					if (!EndPoint.ContainsKey(serviceName))
					{
						var entries = GetService(serviceName).GetAwaiter().GetResult();
						EndPoint.TryAdd(serviceName, entries.Select(p => new ChannelEndPoint
						{
							Address = p.Service.Address,
							Port = p.Service.Port,
							Channel = new Channel(p.Service.Address, p.Service.Port, ChannelCredentials.Insecure),
							Status = ChannelEndPointStatus.Passing
						}).ToList());
						continue;
					}



				}
			}
		}


		public async Task<List<ServiceEntry>> GetService(string serviceId, string tag = "", bool passingOnly = true)
		{
			using (var consul = new ConsulClient(config => { config.Address = new Uri("http://192.168.1.142:8500"); }))
			{
				var result = await consul.Health.Service(serviceId, tag, passingOnly);
				return result.Response.ToList();
			}
		}
	}
}
