

using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Models.Email
{
    public class NewEmailModel
    {

        [SwaggerSchema("The date of the email.")]
        public DateTime EmailDate { get; set; }

        [SwaggerSchema("The email addres to sent email to.")]
        public required string ToEmail { get; set; }

        [SwaggerSchema("The name to send email to.")]
        public required string ToName { get; set; }

        [SwaggerSchema("The subject of the email.")]
        public required string Subject { get; set; }

        [SwaggerSchema("The body of the email.")]
        public string? Body { get; set; }

        [SwaggerSchema("The username of the user.")]
        public string? Username { get; set; }

        [SwaggerSchema("An external reference id link record with external provider records")]
        public string? ExternalRef { get; set; }

    }
}
