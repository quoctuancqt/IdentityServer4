using Core.ContextFactory;
using Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddContext<TContext>(this IServiceCollection services,
            string connectionString) where TContext : DbContext
        {
            services.AddScoped<IContextFactory<TContext>>((serviceProvider) =>
            {
                var user = serviceProvider.GetService<IHttpContextAccessor>().HttpContext.User;

                var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("Debug");

                logger.LogDebug($"Claims: {user?.Claims?.Select(x => x.Type).ToArray()}");

                return new ContextFactory<TContext>(ContextFactory<TContext>.GetDbContext(connectionString, user?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value));
            });

            return services;
        }

        public static void BeforeCommit(this DbContext context, string userId, bool isAudit = true)
        {
            var entriesAdded = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            var entriesModified = context.ChangeTracker.Entries()
                  .Where(e => e.State == EntityState.Modified).Select(e => e.Entity as IAudit);

            if (entriesAdded.Count() > 0) ProcessAudit(entriesAdded, EntityState.Added, userId);

            if (entriesModified.Count() > 0) ProcessAudit(entriesModified, EntityState.Modified, userId);
        }

        private static void ProcessAudit(IEnumerable<object> entries, EntityState state, string userId)
        {
            foreach (var e in entries.Select(e => e as IAudit))
            {
                if (e != null)
                {
                    if (state == EntityState.Added)
                    {
                        e.CreatedBy = userId;
                        e.CreatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        e.UpdatedBy = userId;
                        e.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
