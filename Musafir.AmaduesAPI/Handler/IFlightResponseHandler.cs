using AmadeusWebService;
using Musafir.AmaduesAPI.Response;

namespace Musafir.AmaduesAPI.Handler
{
    public interface IFlightResponseHandler
    {
        AirItineraryInfo[]? GetFlightResonse(Fare_MasterPricerTravelBoardSearchReply reply);
    }
}
