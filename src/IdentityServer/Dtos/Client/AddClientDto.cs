using FluentValidation;
using IdentityServer.Enums;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace IdentityServer.Dtos
{
    public class AddClientDto
    {
        private readonly string _secret;

        private readonly string _clientId;

        public AddClientDto()
        {
            _secret = Guid.NewGuid().ToString();

            _clientId = Guid.NewGuid().ToString();
        }

        public GrantTypeEnum GrantType { get; set; }

        public string ClientName { get; set; }

        public bool AllowAccessTokensViaBrowser => true;

        public bool AllowOfflineAccess => true;

        public bool RequireConsent => false;

        public bool RequirePkce => false;

        public ICollection<string> RedirectUris { get; set; }

        public ICollection<string> PostLogoutRedirectUris { get; set; }

        public ICollection<string> AllowedScopes { get; set; }

        public int AccessTokenLifetime => 70600;

        public int AbsoluteRefreshTokenLifetime => 70600;

        public Client GenerateClient()
        {
            if (AllowOfflineAccess)
            {
                AllowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            }

            return new Client
            {
                ClientId = _clientId,
                ClientName = ClientName,
                AllowedGrantTypes = GetClientGrantTypes(GrantType),
                ClientSecrets =
                {
                    new Secret(_secret.Sha256())
                },
                AllowAccessTokensViaBrowser = IsAllowAccessTokensViaBrowser(GrantType),
                RequireConsent = RequireConsent,
                RedirectUris = RedirectUris,
                PostLogoutRedirectUris = PostLogoutRedirectUris,
                AllowedScopes = AllowedScopes,
                AllowOfflineAccess = AllowOfflineAccess,
                AccessTokenLifetime = AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime,
                Description = _secret,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = RequirePkce,
            };
        }

        private ICollection<string> GetClientGrantTypes(GrantTypeEnum grantTypeEnum)
        {
            return (grantTypeEnum) switch
            {
                GrantTypeEnum.Implicit => GrantTypes.Implicit,
                GrantTypeEnum.Code => GrantTypes.Code,
                GrantTypeEnum.Hybrid => GrantTypes.Hybrid,
                GrantTypeEnum.CodeAndClientCredentials => GrantTypes.CodeAndClientCredentials,
                _ => throw new ArgumentException("Grant type not support")
            };
        }

        private bool IsAllowAccessTokensViaBrowser(GrantTypeEnum grantTypeEnum)
        {
            return (grantTypeEnum) switch
            {
                GrantTypeEnum.Implicit => true,
                GrantTypeEnum.Hybrid => true,
                GrantTypeEnum.CodeAndClientCredentials => true,
                _ => false
            };
        }
    }

    public class AddClientDtoValidator : AbstractValidator<AddClientDto>
    {
        public AddClientDtoValidator()
        {
            RuleFor(x => x.ClientName).NotEmpty();
        }
    }
}
