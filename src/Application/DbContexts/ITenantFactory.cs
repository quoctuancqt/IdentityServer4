using Application.Interfaces;
using DistributedCache.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Application.DbContexts
{
    public interface ITenantFactory
    {
        Task<TenantProfileModel> GetTenantByTenantIdAsync(string id);

        Task CreateAsync(string tenantId);

        Task UpdateAsync(string tenantId);

        T GetTenantContext<T>(string id, ICurrentUser currentUser) where T : DbContext;
    }
}
