using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Client
{
	public class ChannelFactory
	{
		private Dictionary<string, List<ChannelEndPoint>> ServiceEndPoints { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private readonly AsyncLock _asyncLock = new AsyncLock();

		private ILogger Logger { get; }

		public ChannelFactory(GrpcClientConfiguration gRpcClientConfiguration, ILogger<ChannelFactory> logger)
		{
			ServiceEndPoints = new Dictionary<string, List<ChannelEndPoint>>();
			GrpcClientConfiguration = gRpcClientConfiguration;
			Logger = logger;
		}


		public List<Channel> GetChannels(string serviceName)
		{
			if (ServiceEndPoints.TryGetValue(serviceName, out var result))
				return result.Select(p => p.Channel).ToList();
			return new List<Channel>();
		}

		internal async Task RefreshChannels(CancellationToken cancellationToken)
		{
			Logger.LogInformation("Start refresh channel");
			try
			{
				Logger.LogInformation("Waiting lock...");
				using (await _asyncLock.LockAsync(cancellationToken))
				{
					Logger.LogInformation("Get the lock,start refresh...");
					foreach (var serviceName in GrpcClientConfiguration.GrpcServiceName)
					{
						var entries = await GetHealthService(serviceName);
						if (!ServiceEndPoints.ContainsKey(serviceName))
						{
							ServiceEndPoints.Add(serviceName, entries.Select(p => new ChannelEndPoint
							{
								Address = p.Service.Address,
								Port = p.Service.Port,
								Channel = new Channel(p.Service.Address, p.Service.Port,
									ChannelCredentials.Insecure),
								Status = ChannelEndPointStatus.Passing
							}).ToList());
							continue;
						}

						if (ServiceEndPoints.TryGetValue(serviceName, out var endPoints))
						{
							var newEndPoints = entries.FindAll(p =>
								!endPoints.Any(e => e.Address == p.Service.Address && e.Port == p.Service.Port));
							if (newEndPoints.Any())
							{
								endPoints.AddRange(newEndPoints.Select(p => new ChannelEndPoint
								{
									Address = p.Service.Address,
									Port = p.Service.Port,
									Channel = new Channel(p.Service.Address, p.Service.Port,
										ChannelCredentials.Insecure),
									Status = ChannelEndPointStatus.Passing
								}));
							}

							foreach (var entry in newEndPoints)
							{
								var endPoint = endPoints.FirstOrDefault(p =>
									p.Address == entry.Service.Address && p.Port == entry.Service.Port);
								if (endPoint == null)
									endPoints.Add(new ChannelEndPoint
									{
										Address = entry.Service.Address,
										Port = entry.Service.Port,
										Channel = new Channel(entry.Service.Address, entry.Service.Port,
											ChannelCredentials.Insecure),
										Status = ChannelEndPointStatus.Passing
									});
								continue;

							}
						}
					}
				}
				Logger.LogInformation("Refresh channel complete");
			}
			catch (Exception e)
			{
				Logger.LogError(e, "Refresh channel error:{0}", e.Message);
			}
		}


		internal async Task<List<ServiceEntry>> GetHealthService(string serviceId, string tag = "", bool passingOnly = true)
		{
			using (var consul = new ConsulClient(config => { config.Address = new Uri("http://192.168.1.142:8500"); }))
			{
				var result = await consul.Health.Service(serviceId, tag, passingOnly);
				if (result.StatusCode != System.Net.HttpStatusCode.OK)
				{
					Logger.LogError("Get the health service error:{0}", result.StatusCode);
				}
				return result.Response.ToList();
			}
		}
	}
}
