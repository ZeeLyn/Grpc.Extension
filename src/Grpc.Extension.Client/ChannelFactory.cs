using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Extension.Client
{
	internal class ChannelFactory : IChannelFactory
	{
		private Dictionary<string, List<ChannelNode>> ServiceChannels { get; }

		private GrpcClientConfiguration GrpcClientConfiguration { get; }

		private IServiceDiscovery ServiceDiscovery { get; }

		private readonly AsyncLock _asyncLock = new AsyncLock();

		private ILogger Logger { get; }

		public ChannelFactory(GrpcClientConfiguration gRpcClientConfiguration, IServiceDiscovery serviceDiscovery, ILogger<ChannelFactory> logger)
		{
			ServiceChannels = new Dictionary<string, List<ChannelNode>>();
			GrpcClientConfiguration = gRpcClientConfiguration;
			ServiceDiscovery = serviceDiscovery;
			Logger = logger;
		}


		public List<ChannelNode> GetChannelNodes(string serviceName, ChannelNodeStatus status = ChannelNodeStatus.Passing)
		{
			if (!GrpcClientConfiguration.ServicesCredentials.Any(p =>
				 p.Key.Equals(serviceName, StringComparison.CurrentCultureIgnoreCase)))
				throw new InvalidOperationException($"Not found service {serviceName}");
			if (ServiceChannels.TryGetValue(serviceName, out var result))
				return result.FindAll(p => p.Status == status);
			return new List<ChannelNode>();
		}


		public async Task RefreshChannels(CancellationToken cancellationToken)
		{
			Logger.LogInformation("Start refresh channel");
			try
			{
				Logger.LogInformation("Waiting for lock...");
				using (await _asyncLock.LockAsync(cancellationToken))
				{
					Logger.LogInformation("Get the lock,start refresh...");
					foreach (var service in GrpcClientConfiguration.ServicesCredentials)
					{
						if (cancellationToken.IsCancellationRequested)
							break;

						var healthEndPoints = await ServiceDiscovery.GetServices(GrpcClientConfiguration.ConsulClientConfiguration, service.Key, "", true, cancellationToken: cancellationToken);

						if (cancellationToken.IsCancellationRequested)
							break;

						if (ServiceChannels.TryGetValue(service.Key, out var channels))
						{
							foreach (var channel in channels)
							{
								if (cancellationToken.IsCancellationRequested)
									break;
								if (healthEndPoints.Any(p =>
									channel.Address == p.Service.Address && channel.Port == p.Service.Port))
								{
									if (channel.Status == ChannelNodeStatus.Passing) continue;
									channel.Status = ChannelNodeStatus.Passing;
									Logger.LogInformation($"The status of node {channel.Address}:{channel.Port} changes to passing.");
								}
								else
								{
									if (channel.Status == ChannelNodeStatus.Critical)
										continue;
									channel.Status = ChannelNodeStatus.Critical;
									channel.CurrentWeight = 0;
									Logger.LogInformation($"The status of node {channel.Address}:{channel.Port} changes to critical.");
								}
							}

							var newEndPoints = healthEndPoints.FindAll(p =>
								!channels.Any(e => e.Address == p.Service.Address && e.Port == p.Service.Port)).Select(p => new ChannelNode
								{
									Address = p.Service.Address,
									Port = p.Service.Port,
									Channel = new Channel(p.Service.Address, p.Service.Port, service.Value),
									Status = ChannelNodeStatus.Passing,
									Weight = int.Parse(p.Service.Meta.FirstOrDefault(m => m.Key == "X-Weight").Value)
								}).ToList();

							if (newEndPoints.Any())
							{
								channels.AddRange(newEndPoints);
								Logger.LogInformation($"New nodes added:{string.Join(",", newEndPoints.Select(p => p.Address + ":" + p.Port))}");
							}
						}
						else
						{
							ServiceChannels.Add(service.Key, healthEndPoints.Select(p => new ChannelNode
							{
								Address = p.Service.Address,
								Port = p.Service.Port,
								Channel = new Channel(p.Service.Address, p.Service.Port, service.Value),
								Status = ChannelNodeStatus.Passing,
								Weight = int.Parse(p.Service.Meta.FirstOrDefault(m => m.Key == "X-Weight").Value)
							}).ToList());
							Logger.LogInformation($"Discover a new {service.Key} service node.");
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
	}
}
