using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Common.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwashbuckle(this IServiceCollection services,
            IConfiguration configuration,
            Action<SwaggerGenOptions> option = null)
        {

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(configuration.GetValue<string>("Swagger:Version"), new OpenApiInfo
                {
                    Title = configuration.GetValue<string>("Swagger:Title"),
                    Description = configuration.GetValue<string>("Swagger:Description"),
                    Version = configuration.GetValue<string>("Swagger:Version"),
                }
                 );
                option?.Invoke(c);
            });

            return services;
        }

        public static IApplicationBuilder UseSwashbuckle(this IApplicationBuilder app,
            string pathBaseUrl,
            Action<SwaggerUIOptions> option = null)
        {
            app.UseSwagger(c => {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{pathBaseUrl}"
                        }
                    };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{pathBaseUrl}/swagger/v1/swagger.json", "Document API V1");

                option?.Invoke(c);
            });

            return app;
        }
    }
}
