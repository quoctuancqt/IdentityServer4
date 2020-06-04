using DistributedCache.Models;
using IdentityServer.Dtos;
using IdentityServer.Dtos.Base;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public interface IClientService : IClientStore
    {
        Task CreateIdentityResourceAsync();

        Task CreateApiResourceAsync(string[] values);

        Task<string[]> GetApiResourceAsync();

        Task CreateResourceOwnerPasswordAsync(AddClientResourceOwnerPasswordDto dto);

        Task<object> CreateAsync(AddClientDto dto);

        Task<object> UpdateAsync(string clientId, EditClientDto dto);

        Task DeleteAsync(string clientId);

        Task<PageResultDto<Client>> SearchAsync(QuerySearchDefault @param);

        Task<Client> FindEnabledClientByIdAsync(string clientId);

        Task<bool> IsPkceClientAsync(string clientId);

        Task<string> GenerateHeaderCredentialAsync(string clientId);

        Task<TenantProfileModel> RefreshCacheByClientIdAsync(string cliengId);
    }
}
