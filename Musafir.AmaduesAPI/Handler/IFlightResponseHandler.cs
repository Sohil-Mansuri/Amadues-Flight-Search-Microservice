using AmadeusWebService;
using Musafir.AmaduesAPI.Response;

namespace Musafir.AmaduesAPI.Handler
{
    public interface IFlightResponseHandler
    {
        AirItineraryInfo[]? GetFlightResponse(Fare_MasterPricerTravelBoardSearchReply reply);
    }
}
