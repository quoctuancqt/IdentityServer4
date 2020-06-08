using Application.DbContexts;
using DistributedCache.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class ApiBase : ControllerBase
    {
        protected readonly IConfiguration _configuration;

        protected readonly IWebHostEnvironment _hostingEnvironment;

        protected string UserId => User.FindFirst(ClaimTypes.NameIdentifier).Value;

        protected string Email => User.FindFirst(ClaimTypes.Email).Value;

        protected string ClientId => User.FindFirst("client_id").Value;

        protected TenantProfileModel TenantProfile => GetTenantProfile();

        public ApiBase(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostEnvironment;
        }

        private TenantProfileModel GetTenantProfile()
        {
            var tenantFactory = (ITenantFactory)HttpContext.RequestServices.GetService(typeof(ITenantFactory));

            if (string.IsNullOrEmpty(ClientId)) return new TenantProfileModel();

            return tenantFactory.GetTenantByTenantIdAsync(ClientId).GetAwaiter().GetResult();
        }

        protected IActionResult Success() => Ok(true);
    }
}
