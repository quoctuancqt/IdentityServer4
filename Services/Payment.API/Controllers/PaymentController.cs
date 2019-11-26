using Bogus;
using Microsoft.AspNetCore.Mvc;

namespace Payment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var paymentsFaker = new Faker<Payment>()
                .RuleFor(x => x.Name, (f, u) => f.Person.FullName)
                .RuleFor(x => x.Total, (f, u) => f.Finance.Amount());

            return Ok(paymentsFaker.Generate(10));
        }
    }

    public class Payment
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
}
