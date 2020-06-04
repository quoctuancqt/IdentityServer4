using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace IdentityServer.Identity
{
    public class CustomSignInManager : SignInManager<ApplicationUser>
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ApplicationDbContext _dbContext;

        public CustomSignInManager(CustomUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation,
            IIdentityServerInteractionService interaction,
            ApplicationDbContext dbContext)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _interaction = interaction;
            _dbContext = dbContext;
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var request = Context.Request;

            var returnUrl = request.QueryString.Value;

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            var user = await UserManager.FindByNameAsync(userName);

            if (user == null) return SignInResult.Failed;

            var userClient = await _dbContext.UserClients.FirstOrDefaultAsync(x => x.UserId.Equals(user.Id) && x.ClientId.Equals(context.ClientId));

            if (userClient == null) return SignInResult.Failed;

            return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override async Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password,
           bool isPersistent, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var attempt = await CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            return attempt.Succeeded
                ? await SignInOrTwoFactorAsync(user, isPersistent)
                : attempt;
        }

        public override async Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var error = await PreSignInCheck(user);
            if (error != null)
            {
                return error;
            }

            if (await UserManager.CheckPasswordAsync(user, password))
            {
                var alwaysLockout = AppContext.TryGetSwitch("Microsoft.AspNetCore.Identity.CheckPasswordSignInAlwaysResetLockoutOnSuccess", out var enabled) && enabled;
                // Only reset the lockout when TFA is not enabled when not in quirks mode
                if (alwaysLockout || !await IsTfaEnabled(user))
                {
                    await ResetLockout(user);
                }

                return SignInResult.Success;
            }
            Logger.LogWarning(2, "User {userId} failed to provide the correct password.", await UserManager.GetUserIdAsync(user));

            if (UserManager.SupportsUserLockout && lockoutOnFailure)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailedAsync(user);
                if (await UserManager.IsLockedOutAsync(user))
                {
                    return await LockedOut(user);
                }
            }
            return SignInResult.Failed;
        }

        private async Task<bool> IsTfaEnabled(ApplicationUser user)
          => UserManager.SupportsUserTwoFactor &&
          await UserManager.GetTwoFactorEnabledAsync(user) &&
          (await UserManager.GetValidTwoFactorProvidersAsync(user)).Count > 0;
    }
}
