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

            _clientId = Guid.NewGuid().ToString("n");
        }

        public GrantTypeEnum GrantType { get; set; }

        public string ClientName { get; set; }

        public bool AllowAccessTokensViaBrowser => true;

        public bool AllowOfflineAccess => true;

        public bool RequireConsent => false;

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
                AllowedGrantTypes = GrantType == GrantTypeEnum.Implicit ? GrantTypes.Implicit : GrantTypes.Code,
                ClientSecrets =
                {
                    new Secret(_secret.Sha256())
                },
                AllowAccessTokensViaBrowser = GrantType == GrantTypeEnum.Implicit,
                RequireConsent = RequireConsent,
                RedirectUris = RedirectUris,
                PostLogoutRedirectUris = PostLogoutRedirectUris,
                AllowedScopes = AllowedScopes,
                AllowOfflineAccess = AllowOfflineAccess,
                AccessTokenLifetime = AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime,
                Description = _secret
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
