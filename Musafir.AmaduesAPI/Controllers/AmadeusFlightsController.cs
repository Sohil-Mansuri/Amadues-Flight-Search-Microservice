using Microsoft.AspNetCore.Mvc;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Service;

namespace Musafir.AmaduesAPI.Controllers
{
    [Route("api/v1/amadeus")]
    [ApiController]
    public class AmadeusFlightsController(AmadeusFlightService amadeusFlightService) : ControllerBase
    {
        [HttpPost("getFlights")]
        public async Task<IActionResult> GetAmadeusFlight([FromBody] FlightSearchRequestModel requestModel, CancellationToken cancellationToken = default)
        {
            var response = await amadeusFlightService.GetAmaduesFlights(requestModel, cancellationToken);
            return Ok(response);
        }
    }
}
