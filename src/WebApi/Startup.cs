using Application.Interfaces;
using Common.Extensions;
using DistributedCache;
using IdentityServer4.AccessTokenValidation;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //     {
            //         // base-address of your identityserver
            //         options.Authority = Configuration.GetValue<string>("Idsr4:IssuerUri");
            //         options.RequireHttpsMetadata = false;
            //         // name of the API resource
            //         options.Audience = "api_server";
            //     });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetValue<string>("Idsr4:IssuerUri");
                    options.ApiName = Configuration.GetValue<string>("Idsr4:Scope");
                    options.RequireHttpsMetadata = false;
                    options.SupportedTokens = SupportedTokens.Both;
                });

            services.AddSqlServerCache(Configuration);

            services.AddContext();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICurrentUser, CurrentUser>();

            services.AddSwashbuckle(Configuration, option =>
            {
                option.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,

                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetValue<string>("Idsr4:IssuerUri")}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration.GetValue<string>("Idsr4:IssuerUri")}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { Configuration.GetValue<string>("Idsr4:Scope") , "API Server" }
                            },
                        }
                    }
                });

                option.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseSwashbuckle(Configuration.GetValue<string>("PathBaseUrl"), options =>
            {
                options.OAuthUsePkce();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
