using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Extension.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Grpc.Server.WebApp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddScoped<RepertoryService>();
			services.AddGrpcServer(configure =>
			{
				configure.AddServerPort(Configuration.GetSection("GrpcServer:Host").Get<string>(),
					Configuration.GetSection("GrpcServer:Port").Get<int>(), ServerCredentials.Insecure);
				configure.AddConsul(
					client => { client.Address = new Uri("http://192.168.1.142:8500"); },
					service =>
					{
						service.Address = Configuration.GetSection("GrpcServer:Host").Get<string>();
						service.Port = Configuration.GetSection("GrpcServer:Port").Get<int>();
						service.ServiceId =
							$"{Configuration.GetSection("GrpcServer:Host").Get<string>()}:{Configuration.GetSection("GrpcServer:Port").Get<int>()}";
						service.ServiceName = "grpc-server";
						service.HealthCheck = (Configuration.GetSection("GrpcServer:Host").Get<string>(), Configuration.GetSection("Port").Get<int>());
						service.HealthCheckInterval = TimeSpan.FromSeconds(10);
					});
				configure.AddService<HelloService>();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
			app.UseGrpcServer();
		}
	}
}
