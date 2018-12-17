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
	class Program
	{
		static async Task Main(string[] args)
		{
			var list1 = new List<int> { 1, 3, 4 };
			var list2 = new List<int> { 2, 5, 3 };
			System.Console.WriteLine(JsonConvert.SerializeObject(list1.Except(list2)));

			//var c = ChannelFactory.GetService("grpc-server1");
			//System.Console.WriteLine(JsonConvert.SerializeObject(c));
			System.Console.ReadKey();
			//var host = new HostBuilder().ConfigureAppConfiguration((hostingContext, config) =>
			//	{
			//		config.AddCommandLine(args);
			//	})

			//	.ConfigureServices((hostingContext, services) =>
			//	{

			//		services.AddGrpcServer(configure =>
			//		{
			//			configure.ServerPort =
			//				new ServerPort("192.168.1.129", 50051, ServerCredentials.Insecure);
			//			configure.AddService(Hello.BindService(new HelloService()));
			//			configure.AddConsul(consul => { consul.Address = new Uri("http://192.168.1.142:8500"); },
			//				"192.168.1.129", 50051, "grpc-server1", "grpc-server1", new Consul.AgentServiceCheck
			//				{
			//					Interval = TimeSpan.FromSeconds(10),
			//					HTTP = "http://192.168.1.129/api/values",

			//				});

			//		});
			//		//services.AddHostedService();

			//	}).ConfigureHostConfiguration(builder =>
			//	{

			//	}).UseConsoleLifetime().Build().UseGrpcServer();

			//await host.RunAsync();

		}
	}
}
