using Microsoft.AspNetCore.Diagnostics;
using Musafir.AmaduesAPI.Other;

namespace Musafir.AmaduesAPI.Exception
{
    public class GlobalExceptionHandler(IConfiguration configuration, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception, CancellationToken cancellationToken)
        {
            if (configuration["ASPNETCORE_ENVIRONMENT"] == Constants.Development)
            {
                await httpContext.Response.WriteAsJsonAsync(exception, cancellationToken: cancellationToken);
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync("Something went wrong, please contact Admin", cancellationToken: cancellationToken);
            }

            logger.LogError(exception, $"path : {httpContext.Request.Path}");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return true;
        }
    }
}
