using System;
using System.Collections.Generic;
using System.Linq;
using Books.Api.Configuration;
using Books.Api.Middlewares;
using Books.Api.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Books.Api.HealthCheck;
using Books.Api.Services;
using Books.Api.Storage;
using Books.Api.Validation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Books.Api
{
    /// <summary>
    /// Startup.
    /// </summary>
    public class Startup
    {
        private const string ApplicationNameKey = "Serilog:Properties:ApplicationName";
        private readonly string _applicationName;
        private readonly IConfiguration _configuration;
        private readonly Serilog.ILogger _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;

            _logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .Enrich.WithProperty("Version", ReflectionUtils.GetAssemblyVersion<Startup>())
                .CreateLogger();

            _applicationName = _configuration.GetValue<string>(ApplicationNameKey) ?? throw new ArgumentNullException(ApplicationNameKey);
            _logger.Information("Starting {ApplicationName} {ApplicationVersion}", _applicationName, ReflectionUtils.GetAssemblyVersion<Program>());

            if (env.IsDevelopment())
                _logger.Debug(_configuration.Dump());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_logger);

            services.AddHealthChecks()
                .AddCheck<StorageHealthCheck>(nameof(StorageHealthCheck));

            services
                .AddMvcCore(options =>
                {
                    options.Filters.Add<ModelStateValidationFilter>();
                })
                .AddApiExplorer()
                .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            
            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Books Api", Version = "V1" });
            });
            
            services
                //this only will have static data so ok to have singleton lifetime
                .AddSingleton(_configuration.BindTo<StorageSettings>())
                //this will create new connection, every time the Create Method is called so ok to have singleton life time
                .AddSingleton<IDbConnectionFactory, DbConnectionFactory>()
                .AddTransient<IBooksRepository, BooksRepository>()
                .AddTransient<IBooksService,BooksService>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog(_logger);

            var pathBase = _configuration.GetValue<string>("PathBase");

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>(_logger);

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books Api V1"); });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", $"{_applicationName} {ReflectionUtils.GetAssemblyVersion<Program>()}");
            });

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                });
            });

            app.UseSwagger(setupAction =>
                {
                    if (!string.IsNullOrEmpty(pathBase))
                        setupAction.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                            swaggerDoc.Paths = (OpenApiPaths) swaggerDoc.Paths.ToDictionary(path => $"/{pathBase.Trim('/')}{path.Key}", path => path.Value));
                }
            );

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Map("/api/ping", ping => ping.Run(async context => await context.Response.WriteAsync("Pong")));
            app.UseHealthChecks("/api/health");
        }
    }
}
