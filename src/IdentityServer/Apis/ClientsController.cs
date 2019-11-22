using IdentityServer.Dtos;
using IdentityServer.Dtos.Base;
using IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IdentityServer.Apis
{
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

    }
}
