using System;
using FluentValidation;

namespace Product.API.Dtos
{
    public class AddProductDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
    }

    public class AddProductDtoValidator : AbstractValidator<AddProductDto>
    {
        public AddProductDtoValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}
