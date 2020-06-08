using Application.DbContexts;
using Application.Interfaces;
using Domain;
using Domain.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        private readonly ICurrentUser _currentUser;

        public DbSet<Product> Products { get; set; }

        public ApplicationContext(DbContextOptions options, ICurrentUser currentUser) : base(options)
        {
            _currentUser = currentUser;
        }

        public async Task CommitAsync(bool isAudits = true)
        {
            BeforeCommit(isAudits);

            await this.SaveChangesAsync();
        }

        public async Task CommitAsync(Func<Task> action)
        {
            BeforeCommit();

            var strategy = Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = Database.BeginTransaction();

                await SaveChangesAsync();

                await action();

                transaction.Commit();
            });

        }

        private void BeforeCommit(bool isAudits = true)
        {
            var entriesAdded = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            var entriesModified = ChangeTracker.Entries()
                  .Where(e => e.State == EntityState.Modified).Select(e => e.Entity as IAudit);

            if (entriesAdded.Count() > 0) ProcessAudit(entriesAdded, EntityState.Added);

            if (entriesModified.Count() > 0) ProcessAudit(entriesModified, EntityState.Modified);

        }

        private void ProcessAudit(IEnumerable<object> entries, EntityState state)
        {
            foreach (var e in entries.Select(e => e as IAudit))
            {
                if (e != null)
                {
                    if (state == EntityState.Added)
                    {
                        e.CreatedBy = _currentUser.UserId;

                        e.CreatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        e.UpdatedBy = _currentUser.UserId;

                        e.UpdatedAt = DateTime.UtcNow;
                    }

                }
            }
        }
    }
}
