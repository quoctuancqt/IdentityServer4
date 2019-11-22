using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public CustomResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);

            if (user != null)
            {
                var password = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, context.Password);

                if (password == PasswordVerificationResult.Success)
                {
                    context.Result = new GrantValidationResult(user.UserName, "password");
                    return;
                }
            }

            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            return;
        }
    }
}
