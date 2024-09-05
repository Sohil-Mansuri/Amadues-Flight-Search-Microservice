
namespace Musafir.AmaduesAPI.Middleware.IPValidation
{
    public class IPValidationMiddleware : IMiddleware
    {
        private HashSet<string> allowedIPs = [];
        public IPValidationMiddleware(IConfiguration configuration)
        {
            var ips = configuration.GetValue<string>("WhiteListedIPs")?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips?.Length > 0)
            {
                foreach (var ip in ips)
                {
                    allowedIPs.Add(ip);
                }
            }
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var currestUserIP = context.Connection.RemoteIpAddress;

            if (currestUserIP is not null)
            {
                currestUserIP = currestUserIP.MapToIPv4();

                if (!allowedIPs.Contains(currestUserIP.ToString()))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync("please whitelist your IP");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync("something went wrong");
                return;
            }

            await next(context);
        }
    }
}
