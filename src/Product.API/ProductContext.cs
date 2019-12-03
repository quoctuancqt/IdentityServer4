using Core.Extensions;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Product.API.EntityConfigurations;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Product.API
{
    public class ProductContext : DbContext, IUnitOfWork
    {
        private string UserId { get; set; }
        public ProductContext(DbContextOptions options, string userId = null) : base(options)
        {
            UserId = userId;
        }

        public DbSet<Entities.Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.BeforeCommit(UserId);

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }

    public class ProductContextDesignFactory : IDesignTimeDbContextFactory<ProductContext>
    {
        public ProductContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ProductContext>()
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return new ProductContext(optionsBuilder.Options);
        }
    }
}
