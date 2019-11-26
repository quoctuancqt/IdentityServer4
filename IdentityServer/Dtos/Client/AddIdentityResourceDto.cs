namespace IdentityServer.Dtos
{
    using IdentityServer4.Models;
    using System.Collections.Generic;
    using FluentValidation;

    public class AddIdentityResourceDto
    {
        public string Name { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Description { get; set; }

        public IEnumerable<string> ClaimTypes{ get; set; }
        
        public IdentityResource GenerateIdentityResource()
        {
            return new IdentityResource(Name, DisplayName, ClaimTypes)
            {
                Description = Description
            };
        }
    }

    public class AddIdentityResourceDtoValidator : AbstractValidator<AddIdentityResourceDto> 
    {
        public AddIdentityResourceDtoValidator() 
        {
            RuleFor(x => x.Name).NotEmpty();

            RuleFor(x => x.DisplayName).NotEmpty();
        }
    }
}
