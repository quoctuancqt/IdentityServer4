using Microsoft.AspNetCore.Mvc;

namespace Core.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApi : ControllerBase
    {
        protected OkObjectResult Success()
        {
            return Ok(new { success = true });
        }
    }
}
