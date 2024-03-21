using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OKeeffeCraft.Database;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OKeeffeCraft.Authorization
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(Account account);
        public int? ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }

    public class JwtUtils : IJwtUtils
    {
        private readonly DataContext _context;
        private readonly AppSettings _appSettings;

        public JwtUtils(
            DataContext context,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Generates a JWT token for the specified account.
        /// </summary>
        /// <param name="account">The account for which the token is generated.</param>
        /// <returns>The generated JWT token.</returns>
        public string GenerateJwtToken(Account account)
        {
            // Generate a token that is valid for 15 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Validates a JWT token and extracts the account ID if the token is valid.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>The account ID extracted from the JWT token if validation is successful; otherwise, null.</returns>
        public int? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // Set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // Return account id from JWT token if validation successful
                return accountId;
            }
            catch
            {
                // Return null if validation fails
                return null;
            }
        }

        /// <summary>
        /// Generates a refresh token for the specified IP address.
        /// </summary>
        /// <param name="ipAddress">The IP address for which the refresh token is generated.</param>
        /// <returns>The generated refresh token.</returns>
        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshToken = new RefreshToken
            {
                // Token is a cryptographically strong random sequence of values
                Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
                // Token is valid for 7 days
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            // Ensure token is unique by checking against database
            var tokenIsUnique = !_context.Accounts.Any(a => a.RefreshTokens.Any(t => t.Token == refreshToken.Token));

            if (!tokenIsUnique)
                return GenerateRefreshToken(ipAddress);

            return refreshToken;
        }

    }
}
