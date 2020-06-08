using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WebApi
{
    public class CurrentUser : ICurrentUser
    {
        private readonly HttpContext _httpContext;

        public CurrentUser(IHttpContextAccessor _httpContextAccessor)
        {
            _httpContext = _httpContextAccessor.HttpContext;
        }

        public string UserId => _httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string DisplayName => _httpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

        public string Email => _httpContext.User?.FindFirst(ClaimTypes.Email)?.Value;

        public string ClientId => _httpContext.User?.FindFirst("client_id")?.Value;
    }
}
