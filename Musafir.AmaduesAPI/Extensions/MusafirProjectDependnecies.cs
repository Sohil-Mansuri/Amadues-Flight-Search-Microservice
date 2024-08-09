using Musafir.AmaduesAPI.Caching;
using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Header;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Service;

namespace Musafir.AmaduesAPI.Extensions
{
    public static class MusafirProjectDependnecies
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
                options.Configuration = builder.Configuration.GetSection("Redis:ConnectionString").Value;
            });
        }
    }
}
