using AmadeusWebService;
using Musafir.AmaduesAPI.Handler;
using Musafir.AmaduesAPI.Other;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Response;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace Musafir.AmaduesAPI.Service
{
    public class AmadeusFlightService(IConfiguration configuration,
        CustomEndpointBehavior customEndpointBehavior,
        IFightSearchRequestHandler fightSearchRequestHandler,
        IFlightResponseHandler flightResponseHandler,
        FlightCachingHandler flightCachingHandler,
        ILogger<AmadeusFlightService> logger)
    {
        private Master_pricer__PDT_1_0_ServicesPTClient? _amadeusClient;
        private int _timeoutInSeconds = Convert.ToInt32(configuration["AmaduesTimeout"]);


        public Master_pricer__PDT_1_0_ServicesPTClient AmadeusClient
        {
            get
            {
                if (_amadeusClient is null)
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

        public async Task<AirItineraryInfo[]?> GetAmaduesFlights(FlightSearchRequestModel request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //get flights from cache
            var flightsFromCache = await flightCachingHandler.GetFlights(request, cancellationToken);
            if (flightsFromCache?.Length > 0) return flightsFromCache;

            var providerRequest = fightSearchRequestHandler.GetRequest(request);
            var providerResponse = await WithPreAndPostProcessing(providerRequest, AmadeusClient.Fare_MasterPricerTravelBoardSearchAsync, cancellationToken);

            var response = flightResponseHandler.GetFlightResponse(providerResponse.Fare_MasterPricerTravelBoardSearchReply);
            await flightCachingHandler.StoreFlights(response, request, cancellationToken);
            return response;
        }


        private async Task<TResponse> WithPreAndPostProcessing<TRequest, TResponse>(TRequest request,Func<TRequest, Task<TResponse>> action, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var stopwatch = new Stopwatch();
            var timeout = TimeSpan.FromSeconds(_timeoutInSeconds);
            var timeoutTask = Task.Delay(timeout, cancellationToken);
            
            stopwatch.Start();
            var actionTask = action(request);
            var completedTask = await Task.WhenAny(actionTask, timeoutTask);
            if (completedTask == timeoutTask)
            {
                throw new TimeoutException("The operation has timed out.");
            }

            var response = await actionTask;
            stopwatch.Stop();
            var totalTime = stopwatch.Elapsed.TotalMilliseconds;
            logger.LogInformation("Amadues provider response time {totalTime} ms", totalTime);
            return response;
        }
    }
}
