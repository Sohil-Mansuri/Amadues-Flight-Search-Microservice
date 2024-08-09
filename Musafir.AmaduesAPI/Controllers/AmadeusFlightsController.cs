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
        private readonly AmadeusFlightService _amadeusFlightService = amadeusFlightService;

        [HttpPost("getFlights")]
        public async Task<IActionResult> GetAmadeusFlight([FromBody] FlightSearchRequestModel requestModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _amadeusFlightService.GetAmaduesFlights(requestModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, JsonConvert.SerializeObject(ex));
            }
        }
    }
}
