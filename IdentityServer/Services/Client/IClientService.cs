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

        Task CreateApiResourceAsync();

        Task CreateResourceOwnerPasswordAsync(AddClientResourceOwnerPasswordDto dto);

        Task<PageResultDto<Client>> SearchAsync(QuerySearchDefault @param);

        Task<Client> FindEnabledClientByIdAsync(string clientId);

        Task<bool> IsPkceClientAsync(string clientId);

        Task<string> GenerateHeaderCredentialAsync(string clientId);
    }
}
