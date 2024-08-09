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
        private readonly IConfiguration _configuration = configuration;
        private readonly CustomEndpointBehavior _customEndpointBehavior = customEndpointBehavior;
        private readonly IFightSearchRequestHandler _flightSearchRequestHandler = fightSearchRequestHandler;
        private readonly IFlightResponseHandler _flightResponseHandler = flightResponseHandler;
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

                    var amadeusUrl = new Uri(_configuration["AmadeusConfiguration:Url"] ?? string.Empty);
                    _amadeusClient = new Master_pricer__PDT_1_0_ServicesPTClient(binding, new EndpointAddress(amadeusUrl));
                    _amadeusClient.Endpoint.EndpointBehaviors.Add(_customEndpointBehavior);
                }
                return _amadeusClient;
            }
        }

        public async Task<AirItineraryInfo[]?> GetAmaduesFlights(FlightSearchRequestModel request)
        {
            var providerRequest = _flightSearchRequestHandler.GetRequest(request);
            var providerResponse = await AmadeusClient.Fare_MasterPricerTravelBoardSearchAsync(providerRequest);

            var response = _flightResponseHandler.GetFlightResponse(providerResponse.Fare_MasterPricerTravelBoardSearchReply);
            return response;
        }
    }
}
