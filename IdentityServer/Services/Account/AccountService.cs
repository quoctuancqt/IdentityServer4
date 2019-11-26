using System.Threading.Tasks;
using IdentityServer.AutoMapper;
using IdentityServer.Dtos;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(AddUserDto dto)
        {
            var user = dto.ToEntity();

            var result = await _userManager.CreateAsync(user, dto.Password);

            return result;
        }
    }
}
