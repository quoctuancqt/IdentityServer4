using Application.ContextFactory;
using Application.DbContexts;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ApplicationDI
    {
        public static IServiceCollection AddContext(this IServiceCollection services)
        {
            services.AddSingleton<ITenantFactory, TenantFactory>();

            services.AddScoped((serviceProvider) =>
            {
                object factory = serviceProvider.GetService<IHttpContextAccessor>();

                HttpContext context = ((HttpContextAccessor)factory).HttpContext;

                var user = context.User;

                var clientId = user?.FindFirst("client_id")?.Value;

                if (string.IsNullOrEmpty(clientId))
                    throw new ForbiddenException("Invalid client.");

                var tenantFactory = serviceProvider.GetService<ITenantFactory>();

                var currentUser = serviceProvider.GetService<ICurrentUser>();

                return tenantFactory.GetTenantContext<ApplicationContext>(clientId, currentUser);
            });

            return services;
        }
    }
}
