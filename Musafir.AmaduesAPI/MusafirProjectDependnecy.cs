using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Service;

namespace Musafir.AmaduesAPI
{
    public static class MusafirProjectDependnecy
    {
        public static void AddProjectDependencies(this IServiceCollection services)
        {
           services.AddSingleton<AmadeusFlightService>();
           services.AddSingleton<CustomMessageInspector>();
           services.AddSingleton<CustomEndpointBehavior>();
           services.AddSingleton<IFlightResponseHandler, FlightResponseHandler>();
           services.AddSingleton<IFightSearchRequestHandler, FlightSearchRequestHandler>();
           services.AddSingleton<AmadeusSecurityHeader>();
        }


        public static void AddThirdPartyDependnecies(this IServiceCollection services)
        {
          
        }
    }
}
