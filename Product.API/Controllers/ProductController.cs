using Bogus;
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
    }

    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
