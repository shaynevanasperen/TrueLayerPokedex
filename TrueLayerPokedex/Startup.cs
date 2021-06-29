using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrueLayerPokedex.HttpClients;

namespace TrueLayerPokedex
{
	// I would usually modularize this class, but this application is too small to warrant that
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient<PokeApiClient>();
			
			services
				.AddMemoryCache()
				.AddMagneto()
				.WithMemoryCacheStore();
			
			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
