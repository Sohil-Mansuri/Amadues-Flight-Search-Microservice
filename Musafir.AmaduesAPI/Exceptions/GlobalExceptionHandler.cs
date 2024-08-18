using Microsoft.AspNetCore.Diagnostics;
using Musafir.AmaduesAPI.Other;

namespace Musafir.AmaduesAPI.Exceptions
{
    public class GlobalExceptionHandler(IConfiguration configuration, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (configuration["ASPNETCORE_ENVIRONMENT"] == Constants.Development)
            {
                await httpContext.Response.WriteAsJsonAsync(exception, cancellationToken);
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync("Something went wrong, please contact Admin", cancellationToken);
            }

            logger.LogError(exception, "Exception has been happpend");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return true;
        }
    }
}
