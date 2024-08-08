using AmadeusWebService;
using Musafir.AmaduesAPI.Request;

namespace Musafir.AmaduesAPI.Handler
{
    public interface IFightSearchRequestHandler
    {
        Fare_MasterPricerTravelBoardSearchRequest GetRequest(FlightSearchRequestModel request);
    }
}
