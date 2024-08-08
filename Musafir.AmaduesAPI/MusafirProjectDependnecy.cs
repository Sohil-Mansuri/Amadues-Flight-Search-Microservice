using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Service;
using Musafir.DataAccess.AmadeusProvider.Utilities;

namespace Musafir.AmaduesAPI
{
    public static class MusafirProjectDependnecy
    {
        public static void AddProjectDependency(this IServiceCollection services)
        {
           services.AddSingleton<AmadeusFlightService>();
           services.AddSingleton<CustomMessageInspector>();
           services.AddSingleton<CustomEndpointBehavior>();
           services.AddSingleton<IFlightResponseHandler, FlightResponseHandler>();
           services.AddSingleton<IFightSearchRequestHandler, FlightSearchRequestHandler>();
           services.AddSingleton<AmadeusSecurityHeader>();
        }


        public static void AddThirdPartyDependnecy(this IServiceCollection services)
        {

        }
    }
}
