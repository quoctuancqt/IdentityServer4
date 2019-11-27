using HealthChecks.UI.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using System;

namespace ApiGateway
{
    public class Startup
    {
        private IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationProviderKey = "identityserverapikey";

            services.AddHealthChecks()
                .AddCheck("sefl", () => HealthCheckResult.Healthy())
                .AddUrlGroup(new Uri("http://localhost:7001/hc"), "paymentapi-hc", failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri("http://localhost:7002/hc"), "productapi-hc", failureStatus: HealthStatus.Degraded);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(authenticationProviderKey, config =>
            {
                config.Authority = Configuration.GetValue<string>("IdentitySettings:IssuerUri");
                config.ApiName = "api1";
                config.SupportedTokens = SupportedTokens.Jwt;
                config.RequireHttpsMetadata = false;
            });

            services.AddOcelot(Configuration).AddKubernetes();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseAuthorization();
            app.UseAuthentication();

            await app.UseOcelot();
        }
    }
}
