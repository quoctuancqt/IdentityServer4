using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Application.DbContexts
{
    public interface IApplicationContext : IDisposable
    {
        DbSet<Product> Products { get; set; }

        Task CommitAsync(bool isAudits = true);

        Task CommitAsync(Func<Task> action);
    }
}
