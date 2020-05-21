// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer.Identity;
using FluentValidation.AspNetCore;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;
using IdentityServer.Dtos;
using IdentityServer.Extensions;
using IdentityServer4.AccessTokenValidation;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.HttpOverrides;
using IdentityServer.Services;
using IdentityServer.Services.Idsrv4;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddFluentValidation(config => config.RegisterValidatorsFromAssembly(typeof(AddIdentityResourceDto).Assembly));

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddUserManager<CustomUserManager>()
                .AddSignInManager<CustomSignInManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/account/login";
                    options.UserInteraction.LogoutUrl = "/account/logout";
                    options.UserInteraction.ConsentUrl = "/consent/index";
                    options.IssuerUri = Configuration.GetValue<string>("Idsr4:IssuerUri");
                    options.PublicOrigin = Configuration.GetValue<string>("Idsr4:IssuerUri");
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddClientStore<ClientService>()
                .AddRedirectUriValidator<RedirectUriValidator>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                });

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration.GetValue<string>("Idsr4:IssuerUri");
                    options.ApiName = Configuration.GetValue<string>("Idsr4:Scope");
                    options.RequireHttpsMetadata = false;
                    options.SupportedTokens = SupportedTokens.Both;
                });

            services.AddServices();

            services.AddSwashbuckle(Configuration, option =>
            {
                option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetValue<string>("Idsr4:IssuerUri")}/connect/authorize"),
                            Scopes = new Dictionary<string, string> {
                                { Configuration.GetValue<string>("Idsr4:Scope"), "Swagger API" }
                            }
                        }
                    }
                });

                option.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                InitializeDatabase(app);
            }

            app.UseSwashbuckle(Configuration.GetValue<string>("PathBaseUrl"), option =>
            {
                option.OAuthClientId(Configuration.GetValue<string>("Idsr4:ClientId"));
                option.OAuthAppName(Configuration.GetValue<string>("Idsr4:Scope"));
            });

            app.SetPathBaseUrl(Configuration.GetValue<string>("PathBaseUrl"));

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.AddConsoleLifetime<Startup>(loggerFactory);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var appContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                appContext.Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                var passwordHash = serviceScope.ServiceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!appContext.Users.Any())
                {
                    var user = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@yopmail.com",
                        NormalizedUserName = "admin@yopmail.com".ToUpper(),
                        NormalizedEmail = "admin@yopmail.com".ToUpper()
                    };

                    user.PasswordHash = passwordHash.HashPassword(user, "Aa123456");

                    appContext.Users.Add(user);

                    appContext.SaveChanges();
                }
            }
        }
    }
}