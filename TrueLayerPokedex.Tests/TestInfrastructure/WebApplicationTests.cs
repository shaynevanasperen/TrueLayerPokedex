using System;
using System.Threading.Tasks;
using Fun.AspNetCore.Mvc.Testing;
using JustEat.HttpClientInterception;
using MartinCostello.Logging.XUnit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace TrueLayerPokedex.Tests.TestInfrastructure
{
	public abstract class WebApplicationTests : XunitWebApplicationTests<Program>, IAsyncDisposable, IAsyncLifetime
	{
		protected WebApplicationTests(ExtensibleWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
			: base(factory, testOutputHelper)
		{
			if (factory == null) throw new ArgumentNullException(nameof(factory));

			factory.AfterConfigureServices(services =>
			{
				services.AddHttpRequestInterceptor(_ =>
				{
					var options = new HttpClientInterceptorOptions().ThrowsOnMissingRegistration();
					InitializeHttpInterceptions(options);
					return options;
				});
				services.ReplaceLoggerFactoryWithXUnit(testOutputHelper, ConfigureLogger);
			});
		}

		protected HttpClientInterceptorOptions InterceptHttp => App.Services.GetRequiredService<HttpClientInterceptorOptions>();

		static void ConfigureLogger(XUnitLoggerOptions options) => options.ForLevelAndAbove(LogLevel.Information, "System.", "Microsoft.");

		protected virtual void InitializeHttpInterceptions(HttpClientInterceptorOptions options) { }

		public virtual Task InitializeAsync() => Task.CompletedTask;

		async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();

		public virtual ValueTask DisposeAsync() => ValueTask.CompletedTask;
	}
}
