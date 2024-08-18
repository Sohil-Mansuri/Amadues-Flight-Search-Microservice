namespace Musafir.AmaduesAPI.Middleware.IPValidation
{
    public static class IPVWhitelistExtensions
    {
        public static IApplicationBuilder UseIPValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPValidationMiddleware>();
        }
    }
}
