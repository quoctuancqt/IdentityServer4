using Microsoft.EntityFrameworkCore;
using System;

namespace Core.ContextFactory
{
    public class ContextFactory<TContext> :
        IContextFactory<TContext> where TContext : DbContext
    {
        public TContext Context { get; private set; }

        public ContextFactory(TContext context)
        {
            Context = context;
        }

        public static TContext GetDbContext(string connectionString, string userId)
        {
            var options = new DbContextOptionsBuilder<TContext>();

            options.UseNpgsql(connectionString);

            return (TContext)Activator.CreateInstance(typeof(TContext), options.Options, userId);
        }
    }
}
