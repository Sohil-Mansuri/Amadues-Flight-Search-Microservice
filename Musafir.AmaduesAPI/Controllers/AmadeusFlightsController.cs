using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Service;

namespace Musafir.AmaduesAPI.Controllers
{
    [Route("api/v1/amadeus")]
    [ApiController]
    public class AmadeusFlightsController(AmadeusFlightService amadeusFlightService, IValidator<FlightSearchRequestModel> validator) : ControllerBase
    {
        [HttpPost("getFlights")]
        public async Task<IActionResult> GetAmadeusFlight([FromBody] FlightSearchRequestModel requestModel, CancellationToken cancellationToken = default)
        {
            var result = validator.Validate(requestModel);
            if (!result.IsValid)
                return BadRequest(result.Errors);

            var response = await amadeusFlightService.GetAmaduesFlights(requestModel, cancellationToken);
            return Ok(response);
        }
    }
}
