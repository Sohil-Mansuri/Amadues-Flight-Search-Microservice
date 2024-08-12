using Musafir.AmaduesAPI.Caching;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Response;
using System.Text;

namespace Musafir.AmaduesAPI.Handler
{
    public class FlightCachingHandler(ICaching caching)
    {
        public async Task StoreFlights(AirItineraryInfo[]? flights, FlightSearchRequestModel request, CancellationToken cancellationToken)
        {
            var key = GetFlightKey(request);
            await caching.Store(key, flights, cancellationToken);
        }


        public async Task<AirItineraryInfo[]?> GetFlights(FlightSearchRequestModel request, CancellationToken cancellationToken)
        {
            var key = GetFlightKey(request);
            var flights = await caching.GetData<AirItineraryInfo[]?>(key, cancellationToken);
            return flights;
        }



        private string GetFlightKey(FlightSearchRequestModel request)
        {
            StringBuilder key = new();

            foreach (var itinerary in request.Itineraries)
            {
                key.Append($"{itinerary.DepartureAirport}_{itinerary.ArrivalAirport}_{itinerary.DepartureDate}");
            }

            if (request.TotalPax.Adult > 0) key.Append($"A{request.TotalPax.Adult}");

            if (request.TotalPax.Child > 0) key.Append($"C{request.TotalPax.Child}");

            if (request.TotalPax.Infant > 0) key.Append($"I{request.TotalPax.Infant}");


            return key.ToString();

        }
    }
}
