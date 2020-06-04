using Application.ContextFactory;
using Application.Exceptions;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddContext(this IServiceCollection services)
        {
            services.AddSingleton<ITenantFactory, TenantFactory>();

            services.AddScoped((serviceProvider) =>
            {
                object factory = serviceProvider.GetService<IHttpContextAccessor>();

                HttpContext context = ((HttpContextAccessor)factory).HttpContext;

                var user = context.User;

                var clientId = string.Empty;

                if (user == null)
                {
                    clientId = context.Request.Headers["client_id"].ToString();
                }
                else
                {
                    clientId = user.FindFirst("client_id").Value;
                }

                if (string.IsNullOrEmpty(clientId))
                    throw new ForbiddenException("Invalid client.");

                var tenantFactory = serviceProvider.GetService<ITenantFactory>();

                return tenantFactory.GetTenantContext<ApplicationContext>(clientId);
            });

            return services;
        }
    }
}
