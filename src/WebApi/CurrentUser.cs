using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebApi
{
    public class CurrentUser : ICurrentUser
    {
        private readonly HttpContext _httpContext;

        public CurrentUser(IHttpContextAccessor _httpContextAccessor)
        {
            _httpContext = _httpContextAccessor.HttpContext;
        }

        public string UserId => string.Empty;

        public string DisplayName => string.Empty;

        public string Email => string.Empty;

    }
}
