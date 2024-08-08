using Microsoft.AspNetCore.Mvc;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Service;

namespace Musafir.AmaduesAPI.Controllers
{
    [Route("api/amadeus")]
    [ApiController]
    public class AmadeusFlightsController : ControllerBase
    {
        private readonly AmadeusFlightService _amadeusFlightService;
        public AmadeusFlightsController(AmadeusFlightService amadeusFlightService)
        {
            _amadeusFlightService = amadeusFlightService;
        }

        [HttpPost("getFlights")]
        public IActionResult GetAmadeusFlight([FromBody] FlightSearchRequestModel requestModel)
        {
            try
            {
                var response = _amadeusFlightService.GetAmaduesFlights(requestModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
