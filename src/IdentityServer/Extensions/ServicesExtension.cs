using IdentityServer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
