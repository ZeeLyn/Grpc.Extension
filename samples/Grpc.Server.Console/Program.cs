using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Client;
using Grpc.Extension.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Grpc.Server.Console
{
	public struct ServerConfig
	{
		/// <summary>
		/// 初始权重
		/// </summary>
		public int Weight { get; set; }
		/// <summary>
		/// 当前权重
		/// </summary>
		public int Current { get; set; }
		/// <summary>
		/// 服务名称
		/// </summary>
		public string Name { get; set; }
	}

	class Program
	{
		public static int NextServerIndex(ServerConfig[] ss)
		{
			int index = -1;
			int total = 0;
			int size = ss.Count();

			for (int i = 0; i < size; i++)
			{
				ss[i].Current += ss[i].Weight;
				total += ss[i].Weight;

				if (index == -1 || ss[index].Current < ss[i].Current)
				{
					index = i;
				}
			}

			ss[index].Current -= total;
			return index;
		}
		static async Task Main(string[] args)
		{
			var sv = new ServerConfig[] {
				new ServerConfig{ Name="A",Weight=4},
				new ServerConfig{ Name="B",Weight=2},
				new ServerConfig{ Name="C",Weight=1}
			};

			int index = 0;
			int sum = sv.Sum(m => m.Weight);
			for (int i = 0; i < sum; i++)
			{
				index = NextServerIndex(sv);
				System.Console.WriteLine("{0} {1}", sv[index].Name, sv[index].Weight);
			}

			//var c = ChannelFactory.GetService("grpc-server1");
			//System.Console.WriteLine(JsonConvert.SerializeObject(c));
			System.Console.ReadKey();
			IHostBuilder host = new HostBuilder().ConfigureAppConfiguration((hostingContext, config) =>
			{
				config.AddCommandLine(args);
			});

			//.ConfigureServices((hostingContext, services) =>
			//{

			//	services.AddGrpcServer(configure =>
			//	{
			//		configure.ServerPort =
			//			new ServerPort("192.168.1.129", 50051, ServerCredentials.Insecure);
			//		configure.AddService(Hello.BindService(new HelloService()));
			//		configure.AddConsul(consul => { consul.Address = new Uri("http://192.168.1.142:8500"); },
			//			"192.168.1.129", 50051, "grpc-server1", "grpc-server1", new Consul.AgentServiceCheck
			//			{
			//				Interval = TimeSpan.FromSeconds(10),
			//				HTTP = "http://192.168.1.129/api/values",

			//			});

			//	});
			//	//services.AddHostedService();

			//}).ConfigureHostConfiguration(builder =>
			//{

			//}).UseConsoleLifetime().Build().UseGrpcServer();

			await host.UseConsoleLifetime().Build().RunAsync();

		}
	}
}
