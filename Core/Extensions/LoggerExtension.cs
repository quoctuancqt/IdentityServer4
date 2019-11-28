using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Core.Extensions
{
    public static class LoggerExtension
    {
        public static IHostBuilder ConfigureSeriLog(this IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((builder, context) =>
             {
                 var configuration = builder.Configuration;

                 Log.Logger = new LoggerConfiguration()
                     .Enrich.FromLogContext()
                     .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration.GetValue<string>("ElasticConfiguration:Uri")))
                     {
                         AutoRegisterTemplate = true,
                     })
                 .CreateLogger();
             });

            return builder;
        }
    }
}
