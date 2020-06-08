using FluentValidation;
using System.Collections.Generic;

namespace IdentityServer.Dtos
{
    public class EditClientDto
    {
        public ICollection<string> RedirectUris { get; set; }

        public ICollection<string> PostLogoutRedirectUris { get; set; }

        public ICollection<string> AllowedScopes { get; set; }
    }

    public class EditClientDtoValidator : AbstractValidator<EditClientDto> { }
}
