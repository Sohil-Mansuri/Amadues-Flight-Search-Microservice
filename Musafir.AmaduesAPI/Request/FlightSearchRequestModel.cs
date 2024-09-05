
namespace Musafir.AmaduesAPI.Request
{
    public class FlightSearchRequestModel
    {
        public Itinerary[] Itineraries { get; set; } = [];
        public PaxDetails TotalPax { get; set; } = new();
    }

    public class Itinerary
    {
        public string DepartureAirport { get; set; } = string.Empty;

        public string ArrivalAirport { get; set; } = string.Empty;

        public DateTime DepartureDate { get; set; }

    }

    public class PaxDetails
    {
        public byte Adult { get; set; } = 1;

        public byte Child { get; set; }

        public byte Infant { get; set; }

        public int PaxCount => Adult + Child + Infant;
    }


    public enum PaxType
    {
        ADT,
        CHD,
        INF
    }
}
