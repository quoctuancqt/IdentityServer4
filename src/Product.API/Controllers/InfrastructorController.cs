using Core.ContextFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfrastructorController : ControllerBase
    {
        private readonly IContextFactory<ProductContext> _contextFactory;

        public InfrastructorController(IContextFactory<ProductContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        [Route("seed/schema")]
        public IActionResult SeedSchema()
        {
            _contextFactory.Context.Database.Migrate();

            return Ok("Success");
        }
    }
}
