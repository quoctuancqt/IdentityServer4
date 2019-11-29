using Core.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class LoggerExtension
    {
        private const string CONTENT_TYPE = "application/json";

        public static void ConfigureSeriLog(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration.GetValue<string>("ElasticConfiguration:Uri")))
                    {
                        AutoRegisterTemplate = true,
                    })
                .CreateLogger();
        }

        public static IApplicationBuilder AddLog(this IApplicationBuilder app,
            ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");

                    var exception = context.Features.Get<IExceptionHandlerFeature>();

                    var logger = loggerFactory.CreateLogger(typeof(LoggerExtension));

                    if (exception.Error is BadRequestException)
                    {
                        await BadRequest(context, exception);

                        logger.LogError(exception.GetErrorMessage());

                        return;
                    }

                    if (exception.Error is ForbiddenException)
                    {
                        context.Response.StatusCode = 403;

                        string errorMsg = string.IsNullOrEmpty(exception.Error.Message) ? "Forbidden" : exception.Error.Message;

                        await context.Response.WriteAsync(errorMsg).ConfigureAwait(false);

                        logger.LogError(exception.GetErrorMessage());

                        return;
                    }

                    context.Response.StatusCode = 500;

                    context.Response.ContentType = "application/json";

                    var error = JsonConvert.SerializeObject(new
                    {
                        error = exception.Error.Message,
                        stackTrace = exception.Error.StackTrace,
                    });

                    await context.Response.WriteAsync(error).ConfigureAwait(false);

                    logger.LogError(exception.GetErrorMessage());

                    return;

                });
            });

            return app;
        }

        private static async Task BadRequest(HttpContext context, IExceptionHandlerFeature exception)
        {
            context.Response.StatusCode = 400;

            context.Response.ContentType = CONTENT_TYPE;

            var badRequestException = exception.Error as BadRequestException;

            if (badRequestException.Errors == null)
            {
                var error = JsonConvert.SerializeObject(new { error = exception.Error.Message });

                await context.Response.WriteAsync(error).ConfigureAwait(false);
            }
            else
            {
                var values = badRequestException.Errors.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key.FirstCharToLower(), d.Value));

                await context.Response.WriteAsync("{" + string.Join(",", values) + "}").ConfigureAwait(false);
            }
        }

        private static string FirstCharToLower(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            return input.First().ToString().ToLower() + input.Substring(1);
        }

        private static string GetErrorMessage(this IExceptionHandlerFeature exception)
        {
            return JsonConvert.SerializeObject(new { error = exception.Error.Message });
        }
    }
}
