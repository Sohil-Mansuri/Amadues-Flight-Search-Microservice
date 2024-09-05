using Musafir.AmaduesAPI.Caching;
using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Header;
using Musafir.AmaduesAPI.Middleware.IPValidation;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Service;
using Serilog.Filters;
using Serilog;
using Serilog.Formatting.Compact;

namespace Musafir.AmaduesAPI
{
    public static class DependencyInjection
    {
        public static void AddProjectDependencies(this IServiceCollection services)
        {
            services.AddScoped<AmadeusFlightService>();
            services.AddScoped<CustomMessageInspector>();
            services.AddScoped<CustomEndpointBehavior>();
            services.AddScoped<IFlightResponseHandler, FlightResponseHandler>();
            services.AddScoped<IFightSearchRequestHandler, FlightSearchRequestHandler>();
            services.AddScoped<AmadeusSecurityHeader>();

            services.AddSingleton<ICaching, RedisCachingService>();
            services.AddSingleton<FlightCachingHandler>();
        }


        public static void AddThirdPartyDependnecies(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "AmadeusDemo";
            });
        }


        public static void AddCustomMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<IPValidationMiddleware>();
        }


        public static void AddSerilog(this IServiceCollection services, WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProcessId()
            .WriteTo.Console()
            .WriteTo.Logger(config =>
            {
                config
                   .Filter.ByExcluding(evt => evt.Properties.ContainsKey("Logger") && evt.Properties["Logger"].ToString() == "\"Amadeus\"")
                   .WriteTo.File(new CompactJsonFormatter(), "Logs/Application/log-.txt", rollingInterval: RollingInterval.Day);
            })
            .WriteTo.Logger(config =>
            {
                config
                   .Filter.ByIncludingOnly(evt => evt.Properties.ContainsKey("Logger") && evt.Properties["Logger"].ToString() == "\"Amadeus\"")
                   .WriteTo.File(new CompactJsonFormatter(), "Logs/Amadeus/log-.txt", rollingInterval: RollingInterval.Day);
            })
            .WriteTo.Seq("http://localhost:8081")
            .Filter.ByExcluding(Matching.FromSource("Microsoft"))
            .Filter.ByExcluding(Matching.FromSource("System"))
            .CreateLogger();

            builder.Host.UseSerilog();

            var subLogger = Log.Logger.ForContext("Logger", "Amadeus");

            builder.Services.AddSingleton(subLogger);
        }
    }
}
