using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OKeeffeCraft.Database;
using OKeeffeCraft.Helpers;

namespace OKeeffeCraft.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
        }

        /// <summary>
        /// Middleware for JWT token validation and attaching the associated account to the HttpContext.
        /// </summary>
        /// <param name="context">The HttpContext for the current request.</param>
        /// <param name="dataContext">The data context for accessing the database.</param>
        /// <param name="jwtUtils">The service for JWT token utilities.</param>
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
                        // Attach account to context on successful JWT validation
                        context.Items["Account"] = await dataContext.Accounts.FindAsync(accountId.Value);
                    }
                }
            }
            await _next(context);
        }

    }
}
