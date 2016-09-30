

namespace KnockoutMVC
{

	using System.Diagnostics.CodeAnalysis;

	using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class Startup
	{
		public Startup(IHostingEnvironment hostEnv)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(hostEnv.ContentRootPath)
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				 .AddJsonFile($"appsettings.{hostEnv.EnvironmentName}.json", optional: true)
				 .AddEnvironmentVariables();
			Configuration = builder?.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services?.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder appBuilder, IHostingEnvironment hostEnv, ILoggerFactory loggerFactory)
		{
			loggerFactory?.AddConsole(Configuration?.GetSection("Logging"));
			loggerFactory?.AddDebug();
			var httpContextAccessor = appBuilder?.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
			Extensions.UrlHelperExtensions.Configure(httpContextAccessor);

			if ( hostEnv.IsDevelopment() )
			{
				appBuilder?.UseDeveloperExceptionPage();
				appBuilder?.UseBrowserLink();
			}
			else
				appBuilder?.UseExceptionHandler("/Home/Error");

			appBuilder?.UseStaticFiles();

			appBuilder?.UseMvc
			(routes =>
			 {
				 routes.MapRoute
				 (
					 name: "default",
					 template: "{controller=Home}/{action=Index}/{id?}");
			 });
		}
	}
}
