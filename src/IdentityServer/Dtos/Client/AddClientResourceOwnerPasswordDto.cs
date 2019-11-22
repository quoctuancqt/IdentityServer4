namespace IdentityServer.Dtos
{
    using FluentValidation;
    using IdentityServer4;
    using IdentityServer4.Models;
    using System;
    using System.Collections.Generic;
    using System.Text;

    //http://docs.identityserver.io/en/latest/quickstarts/8_aspnet_identity.html
    public class AddClientResourceOwnerPasswordDto
    {
        private readonly string _secret;

        private readonly string _clientId;

        public AddClientResourceOwnerPasswordDto() 
        {
            _secret = Guid.NewGuid().ToString();

            _clientId = Guid.NewGuid().ToString("n");

            AccessTokenLifetime = 70600;

            AbsoluteRefreshTokenLifetime = 70600;
        }
        
        public string ClientName { get; set; }

        public ICollection<string> AllowedScopes { get; set; }

        public bool AllowOfflineAccess { get; set; } = true;

        public int AccessTokenLifetime { get; set; }

        public int AbsoluteRefreshTokenLifetime { get; set; }

        public IdentityServer4.Models.Client GenerateClient() 
        {
            if (AllowOfflineAccess) 
            {
                AllowedScopes.Add(IdentityServerConstants.StandardScopes.OfflineAccess);
            }
            
            return new IdentityServer4.Models.Client
            {
                ClientId = _clientId,
                ClientName = ClientName,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                Description = _secret,
                ClientSecrets =
                {
                    new Secret(_secret.Sha256())
                },
                AllowedScopes = AllowedScopes,
                AllowOfflineAccess = AllowOfflineAccess,
                AccessTokenLifetime = AccessTokenLifetime,
                AbsoluteRefreshTokenLifetime = AbsoluteRefreshTokenLifetime,
            };
        }
    }

    public class AddClientResourceOwnerPasswordDtoValidator : AbstractValidator<AddClientResourceOwnerPasswordDto> 
    {
        public AddClientResourceOwnerPasswordDtoValidator() 
        {
            RuleFor(x => x.ClientName).NotEmpty();

            RuleFor(x => x.AbsoluteRefreshTokenLifetime).Must(x => x > 0);

            RuleFor(x => x.AccessTokenLifetime).Must(x => x > 0);
        }
    }
}
