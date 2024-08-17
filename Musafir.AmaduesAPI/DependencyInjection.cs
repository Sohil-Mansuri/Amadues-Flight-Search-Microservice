using Musafir.AmaduesAPI.Caching;
using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Header;
using Musafir.AmaduesAPI.Middleware.IPValidation;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Service;

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
            });
        }


        public static void AddCustomMiddlewares(this IServiceCollection services)
        {
            services.AddTransient<IPValidationMiddleware>();
        }
    }
}
