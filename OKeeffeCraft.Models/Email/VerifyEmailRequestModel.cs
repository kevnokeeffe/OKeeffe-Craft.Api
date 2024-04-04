using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Models.Email
{
    public class VerifyEmailRequestModel
    {
        [SwaggerSchema("The verify email request token")]
        public required string Token { get; set; }
    }
}
