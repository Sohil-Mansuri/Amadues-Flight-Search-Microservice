using FluentAssertions;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Response;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http.Json;

namespace Musafir.Test.IntegrationTest
{
    [TestFixture]
    internal class AmadeusFlightControllerTesting
    {
        [Test]
        public async Task Shoud_Get_Amadeus_Flights()
        {
            var application = new AmadeusFlightFactory();

            var client = application.CreateClient();

            var request = new FlightSearchRequestModel
            {
                Itineraries =
                [
                    new Itinerary
                    {
                        ArrivalAirport = "DXB",
                        DepartureAirport = "BOM",
                        DepartureDate = DateTime.Now.AddDays(10)
                    }
                ],
                TotalPax = new PaxDetails
                {
                    Adult = 1
                }
            };

            var response = await client.PostAsJsonAsync("api/v1/amadeus/getFlights", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AirItineraryInfo[]?>();
            result.Should().NotBeNull().And.NotBeEmpty();

        }
    }
}
