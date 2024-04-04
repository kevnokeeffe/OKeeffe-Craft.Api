using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Models.Email
{
    public class ConfirmEmailModel
    {
        [SwaggerSchema("The email address of the user")]
        public required string Email { get; set; }

        [SwaggerSchema("The full name of the user")]
        public string? Name { get; set; }

        [SwaggerSchema("The confirmation access token")]
        public string? AccessToken { get; set; }

        [SwaggerSchema("The username of the user")]
        public string? Username { get; set; }

    }
}
