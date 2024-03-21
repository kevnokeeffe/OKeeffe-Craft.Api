using Microsoft.AspNetCore.Http;
using OKeeffeCraft.Authorization.Interfaces;
using System.Security.Claims;

namespace OKeeffeCraft.Authorization.Services
{
    public class AuthIdentityService : IAuthIdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool HasRole(string role)
        {
            var roles = _httpContextAccessor.HttpContext!.User?.Claims;
            bool result = roles!.Any(p => p.Type == ClaimTypes.Role && p.Value == role);
            return result;
        }

        public string? GetAccountId() => _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
