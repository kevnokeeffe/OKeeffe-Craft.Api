using System.Text.Json.Serialization;

namespace OKeeffeCraft.Models.Accounts
{
    public class AuthenticateResponse
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
        public string? JwtToken { get; set; }

        // [JsonIgnore] // refresh token is returned in http only cookie
        public string? RefreshToken { get; set; }
    }
}
