using Microsoft.AspNetCore.Mvc;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Service;
using Newtonsoft.Json;

namespace Musafir.AmaduesAPI.Controllers
{
    [Route("api/v1/amadeus")]
    [ApiController]
    public class AmadeusFlightsController(AmadeusFlightService amadeusFlightService) : ControllerBase
    {
        [HttpPost("getFlights")]
        public async Task<IActionResult> GetAmadeusFlight([FromBody] FlightSearchRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await amadeusFlightService.GetAmaduesFlights(requestModel);
            return Ok(response);
        }
    }
}
