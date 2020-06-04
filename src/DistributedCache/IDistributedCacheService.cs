using Microsoft.Extensions.Caching.Distributed;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedCache
{
    public interface IDistributedCacheService
    {
        T Get<T>(string key) where T : class;
        
        Task<T> GetAsync<T>(string key, CancellationToken token = default(CancellationToken)) where T : class;
        
        void Refresh(string key);
        
        Task RefreshAsync(string key, CancellationToken token = default(CancellationToken));
        
        void Remove(string key);
        
        Task RemoveAsync(string key, CancellationToken token = default(CancellationToken));
        
        void Set<T>(string key, T value, DistributedCacheEntryOptions options = null) where T : class;

        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options = null, CancellationToken token = default(CancellationToken)) where T : class;

    }
}
