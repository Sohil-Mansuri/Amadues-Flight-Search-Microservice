using AmadeusWebService;
using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Response;
using System.ServiceModel;

namespace Musafir.AmaduesAPI.Service
{
    public class AmadeusFlightService(IConfiguration configuration, 
        CustomEndpointBehavior customEndpointBehavior, 
        IFightSearchRequestHandler fightSearchRequestHandler, 
        IFlightResponseHandler flightResponseHandler)
    {
        private Master_pricer__PDT_1_0_ServicesPTClient? _amadeusClient;


        public Master_pricer__PDT_1_0_ServicesPTClient AmadeusClient
        {
            get
            {
                if (_amadeusClient == null)
                {
                    var binding = new BasicHttpsBinding
                    {
                        MaxReceivedMessageSize = 50000000,
                        ReceiveTimeout = TimeSpan.FromSeconds(180)
                    };

                    var amadeusUrl = new Uri(configuration["AmadeusConfiguration:Url"] ?? string.Empty);
                    _amadeusClient = new Master_pricer__PDT_1_0_ServicesPTClient(binding, new EndpointAddress(amadeusUrl));
                    _amadeusClient.Endpoint.EndpointBehaviors.Add(customEndpointBehavior);
                }
                return _amadeusClient;
            }
        }

        public async Task<AirItineraryInfo[]?> GetAmaduesFlights(FlightSearchRequestModel request)
        {
            var providerRequest = fightSearchRequestHandler.GetRequest(request);
            var providerResponse = await AmadeusClient.Fare_MasterPricerTravelBoardSearchAsync(providerRequest);

            var response = flightResponseHandler.GetFlightResponse(providerResponse.Fare_MasterPricerTravelBoardSearchReply);
            return response;
        }
    }
}
