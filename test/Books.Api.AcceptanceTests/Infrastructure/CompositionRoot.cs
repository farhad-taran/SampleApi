using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace Books.Api.AcceptanceTests.Infrastructure
{
    /// <summary>
    /// shared class that is initialized and run only once amongst all tests using CollectionFixture
    /// </summary>
    public class CompositionRoot : WebApplicationFactory<Startup>
    {
        protected readonly IServiceProvider ServiceProvider;
        public HttpClient HttpClient { get; }

        public CompositionRoot()
        {
            var appFactory = WithWebHostBuilder(config =>
            {
                config
                    .ConfigureTestServices(services =>
                    {
                        services.AddSingleton<StorageFacade>();
                        services.Replace(ServiceDescriptor.Singleton<ILogger>(sc => new LoggerConfiguration()
                            .WriteTo
                            .Console()
                            .CreateLogger()));
                    });
            });
            HttpClient = appFactory.CreateClient();
            ServiceProvider = appFactory.Server.Host.Services;
        }
        
        public T GetRequiredService<T>() where T : class
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }
}
