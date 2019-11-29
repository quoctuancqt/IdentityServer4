using Bogus;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            var productsFaker = new Faker<Product>()
                .RuleFor(x => x.Name, (f, u) => f.Commerce.ProductName())
                .RuleFor(x => x.Price, (f, u) => decimal.Parse(f.Commerce.Price()));

            return Ok(productsFaker.Generate(10));
        }

        [HttpGet("bad-request")]
        public IActionResult BadRequestResult()
        {
            throw new BadRequestException("Bad request from Product API");
        }
    }

    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
