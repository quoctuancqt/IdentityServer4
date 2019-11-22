using IdentityServer.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> CreateUserAsync(AddUserDto dto);
    }
}
