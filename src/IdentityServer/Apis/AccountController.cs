using IdentityServer.Dtos;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityServer.Apis
{
    public class AccountController : ApiBase<AccountController>
    {
        private readonly IAccountService _accountService;
        public AccountController(ILogger<AccountController> logger,
            IAccountService accountService) : base(logger)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] AddUserDto addUserDto)
        {
            var result = await _accountService.CreateUserAsync(addUserDto);

            if (result.Succeeded) return Ok(result);

            return BadRequest(result.Errors);
        }
    }
}
