using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Core.Extensions
{
    public class LoggerExtension
    {
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
    }
}
