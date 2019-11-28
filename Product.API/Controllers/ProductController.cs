using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

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
            try
            {
                throw new Exception("Bad request from Product API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Ok();
        }
    }

    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}
