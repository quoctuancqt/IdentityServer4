using Microsoft.EntityFrameworkCore;

namespace Core.ContextFactory
{
    public interface IContextFactory<TContext> where TContext : DbContext
    {
        TContext Context { get; }
    }
}
