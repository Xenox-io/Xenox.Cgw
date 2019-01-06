using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xenox.AspNetCore.WebSockets.HttpJsonRpc;
using Xenox.Command.Gateway.Host.DependencyInjection;

namespace Xenox.Command.Gateway.Host.HttpJsonRpc {
	public class Startup {
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services) {
			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json")
				.Build()
			;
			services.AddCors();
			services.AddJsonRpcWithWebSocketsSupport(config => {
				config.ShowServerExceptions = true;
			});
			services.AddCgw(configuration);
		}

		public void Configure(IApplicationBuilder application, IHostingEnvironment environment) {
			application.UseCors(
				options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
			);
			application.UseJsonRpcWithWebSocketsSupport();
		}
	}
}
