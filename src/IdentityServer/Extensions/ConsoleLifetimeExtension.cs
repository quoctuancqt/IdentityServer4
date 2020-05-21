using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace IdentityServer.Extensions
{
    public static class ConsoleLifetimeExtension
    {
        public static void AddConsoleLifetime<T>(this IApplicationBuilder app, ILoggerFactory logFactory,
            Action<IServiceProvider> action = null)
            where T : class
        {
            var hostApplicationLifetime = app
                .ApplicationServices
                .GetService(typeof(IHostApplicationLifetime)) as IHostApplicationLifetime;

            var configuration = app
                .ApplicationServices
                .GetService(typeof(IConfiguration)) as IConfiguration;

            var logger = logFactory.CreateLogger<T>();

            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                logger.LogInformation("Application started. Press Ctrl+C to shut down.");

                logger.LogInformation("Hosting environment: {environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

                logger.LogInformation("Content root path: {currentDirectory}", Environment.CurrentDirectory);

                logger.LogInformation("Now listening on: {ASPNETCORE_URLS}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));

                action?.Invoke(app.ApplicationServices);
            });

            hostApplicationLifetime.ApplicationStopped.Register(() =>
            {
                logger.LogInformation("Application is shutting down...");
            });
        }
    }
}
