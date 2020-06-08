using Application.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TenantController : ApiBase
    {
        private readonly ITenantFactory _tenantFactory;
        private readonly ILogger _logger;

        public TenantController(IConfiguration configuration, IWebHostEnvironment hostEnvironment,
            ITenantFactory tenantFactory, ILoggerFactory loggerFactory) : base(configuration, hostEnvironment)
        {
            _tenantFactory = tenantFactory;
            _logger = loggerFactory.CreateLogger(typeof(TenantController));
        }

        [HttpPost("create/{tenantId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public async Task<IActionResult> Create(string tenantId)
        {
            try
            {
                await _tenantFactory.CreateAsync(tenantId);

                return Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                throw ex;
            }

        }

        [HttpPut("update/{tenantId}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public async Task<IActionResult> Update(string tenantId)
        {
            await _tenantFactory.UpdateAsync(tenantId);

            return Success();
        }
    }
}
