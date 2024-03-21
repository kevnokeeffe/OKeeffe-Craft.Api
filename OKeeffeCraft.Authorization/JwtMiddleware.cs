using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OKeeffeCraft.Database;
using OKeeffeCraft.Helpers;

namespace OKeeffeCraft.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, DataContext dataContext, IJwtUtils jwtUtils)
        {
            // Check if Authorization header is present and not null
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (token != null)
                {
                    var accountId = jwtUtils.ValidateJwtToken(token);
                    if (accountId != null)
                    {
                        // attach account to context on successful jwt validation
                        context.Items["Account"] = await dataContext.Accounts.FindAsync(accountId.Value);
                    }
                }
            }
            await _next(context);
        }
    }
}
