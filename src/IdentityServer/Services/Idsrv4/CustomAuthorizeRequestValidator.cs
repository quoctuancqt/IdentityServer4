using IdentityModel;
using IdentityServer.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Services.Idsrv4
{
    public class CustomAuthorizeRequestValidator : ICustomAuthorizeRequestValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenValidator _validator;

        public CustomAuthorizeRequestValidator(UserManager<ApplicationUser> userManager, ITokenValidator validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task ValidateAsync(CustomAuthorizeRequestValidationContext context)
        {
            var userToken = context.Result.ValidatedRequest.Raw.Get("token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.UnauthorizedClient;
                return;
            }

            var result = await _validator.ValidateAccessTokenAsync(userToken);

            var sub = result.Claims.FirstOrDefault(c => c.Type == "sub").Value;

            var user = await _userManager.FindByIdAsync(sub);

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Any(x => x.Equals("SuperAdministrator")))
            {
                context.Result.IsError = true;
                context.Result.Error = OidcConstants.TokenErrors.UnauthorizedClient;
                return;
            }
        }
    }
}
