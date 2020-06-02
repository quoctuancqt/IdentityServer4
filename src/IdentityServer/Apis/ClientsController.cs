using IdentityServer.Dtos;
using IdentityServer.Dtos.Base;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityServer.Apis
{
    [Authorize]
    public class ClientsController : ApiBase<ClientsController>
    {

        private readonly IClientService _clientService;

        public ClientsController(ILogger<ClientsController> logger,
            IClientService clientService) : base(logger)
        {
            _clientService = clientService;
        }

        [HttpGet("clientId")]
        public async Task<IActionResult> GetByClientId(string clientId)
        => Ok(await _clientService.FindClientByIdAsync(clientId));

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery]QuerySearchDefault @param)
        => Ok(await _clientService.SearchAsync(@param));

        #region ResourceOwnerPassword

        [HttpPost("resource-owner-password")]
        public async Task<IActionResult> CreateResourceOwnerPassword([FromBody] AddClientResourceOwnerPasswordDto dto)
        {
            await _clientService.CreateResourceOwnerPasswordAsync(dto);

            return Success();
        }

        [HttpPost("resource-owner-password/credential/{clientId}")]
        public async Task<IActionResult> GenerateHeaderCredentialAsync(string clientId)
        => Ok(await _clientService.GenerateHeaderCredentialAsync(clientId));

        #endregion ResourceOwnerPassword

        #region ImplicitAndHybrid

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] AddClientDto dto)
        {
            var result = CheckValidation(dto);

            if (!result.IsValid) return BadRequest(result.Errors);

            return Ok(await _clientService.CreateAsync(dto));
        }

        [HttpPut("{clientId}")]
        public async Task<IActionResult> Update(string clientId, [FromBody] EditClientDto dto)
        {
            var result = CheckValidation(dto);

            if (!result.IsValid) return BadRequest(result.Errors);

            return Ok(await _clientService.UpdateAsync(clientId, dto));
        }


        [HttpDelete("{clientId}")]
        public async Task<IActionResult> Delete(string clientId)
        {
            await _clientService.DeleteAsync(clientId);

            return Success();
        }
        #endregion ImplicitAndHybrid

        #region Scopes

        [HttpPost("scope/{value}")]
        public async Task<IActionResult> AddScope(string value)
        {
            await _clientService.CreateApiResourceAsync(new string[] { value });

            return Success();
        }

        [HttpGet("scopes")]
        public async Task<IActionResult> GetScopes()
            => Ok(await _clientService.GetApiResourceAsync());

        #endregion Scopes
    }
}
