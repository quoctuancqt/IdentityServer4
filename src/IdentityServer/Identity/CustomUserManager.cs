using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace IdentityServer.Identity
{
    public class CustomUserManager : UserManager<ApplicationUser>
    {
        private readonly ApplicationDbContext _dbContext;
        public CustomUserManager(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger,
            ApplicationDbContext dbContext)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Claim>> GetClaimsAsync(string userId)
        {
            var user = await FindByIdAsync(userId);

            return await GetClaimsAsync(user);
        }

        public async Task<IdentityResult> SendMailForgotPasswordAsync(ApplicationUser user)
        {
            var token = HttpUtility.UrlEncode(await GeneratePasswordResetTokenAsync(user));

            //await _emailService.SendAsync(new Message
            //{
            //    Subject = "[www.c-sharp.vn] Reset password",
            //    Emails = new string[] { user.Email },
            //    HtmlContent = HtmlParser.GenerateBody(new ForgotPasswordModel(_issuerUri, user.Id, token))
            //});

            return await SetLockoutEnabledAsync(user, true);
        }

        public async Task<IdentityResult> SendMailActivationAsync(ApplicationUser user)
        {
            var token = HttpUtility.UrlEncode(await GenerateEmailConfirmationTokenAsync(user));

            //await _emailService.SendAsync(new Message
            //{
            //    Subject = "[www.c-sharp.vn] Activation",
            //    Emails = new string[] { user.Email },
            //    HtmlContent = HtmlParser.GenerateBody(new ActivationModel(_issuerUri, user.Id, token)
            //    {
            //        UserName = user.UserName
            //    })
            //});

            return IdentityResult.Success;
        }

        public override async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var result = await base.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded) return result;

            return await SetLockoutEnabledAsync(user, false);
        }

        public override async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            var result = await base.ConfirmEmailAsync(user, token);

            if (!result.Succeeded) return result;

            user.ConcurrencyStamp = Guid.NewGuid().ToString("n");

            user.LockoutEnabled = false;

            return await UpdateAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password, string clientId)
        {
            var result = await base.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await base.AddToRoleAsync(user, "NormalUser");

                _dbContext.UserClients.Add(new UserClient { ClientId = clientId, UserId = user.Id });

                await _dbContext.SaveChangesAsync();

                await SendMailActivationAsync(user);
            }

            return result;
        }
    }
}
