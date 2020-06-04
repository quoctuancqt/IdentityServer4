using DistributedCache.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Application.ContextFactory
{
    public interface ITenantFactory
    {
        Task<TenantProfileModel> GetTenantByTenantIdAsync(string id);

        Task CreateAsync(string tenantId);

        Task UpdateAsync(string tenantId);

        T GetTenantContext<T>(string id) where T : DbContext;
    }
}
