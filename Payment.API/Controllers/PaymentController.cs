using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Payment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var paymentsFaker = new Faker<Payment>()
                .RuleFor(x => x.Name, (f, u) => f.Person.FullName)
                .RuleFor(x => x.Total, (f, u) => f.Finance.Amount());

            return Ok(paymentsFaker.Generate(10));
        }

        [HttpGet("bad-request")]
        public IActionResult BadRequestResult()
        {
            try
            {
                throw new Exception("Bad request from Payment API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Ok();
        }
    }

    public class Payment
    {
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
}
