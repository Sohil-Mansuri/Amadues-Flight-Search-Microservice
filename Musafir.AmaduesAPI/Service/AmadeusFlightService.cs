using AmadeusWebService;
using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Response;
using System.Globalization;
using System.ServiceModel;

namespace Musafir.AmaduesAPI.Service
{
    public class AmadeusFlightService
    {
        private readonly IConfiguration _configuration;
        private readonly CustomEndpointBehavior _customEndpointBehavior;
        private readonly IFightSearchRequestHandler _flightSearchRequestHandler;
        private readonly IFlightResponseHandler _flightResponseHandler;
        private Master_pricer__PDT_1_0_ServicesPTClient? _amadeusClient;


        public AmadeusFlightService(IConfiguration configuration, 
          CustomEndpointBehavior customEndpointBehavior,
          IFightSearchRequestHandler fightSearchRequestHandler, 
          IFlightResponseHandler flightResponseHandler)
        {
            _configuration = configuration;
            _customEndpointBehavior = customEndpointBehavior;
            _flightSearchRequestHandler = fightSearchRequestHandler;
            _flightResponseHandler = flightResponseHandler;
        }

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


        public AirItineraryInfo[]? GetAmaduesFlights(FlightSearchRequestModel request)
        {
            var providerRequest = _flightSearchRequestHandler.GetRequest(request);
            var providerResponse = AmadeusClient.Fare_MasterPricerTravelBoardSearch(providerRequest);

            var response = _flightResponseHandler.GetFlightResonse(providerResponse.Fare_MasterPricerTravelBoardSearchReply);
            return response;
        }
    }
}
