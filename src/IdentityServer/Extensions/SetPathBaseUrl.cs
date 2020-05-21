using Microsoft.AspNetCore.Builder;

namespace IdentityServer.Extensions
{
    public static class BasePathUrlExtension
    {
        public static IApplicationBuilder SetPathBaseUrl(this IApplicationBuilder app, string pathBaseUrl)
        {
            app.Use((context, next) =>
            {
                context.Request.PathBase = pathBaseUrl;
                return next();
            });

            return app;
        }
    }
}
