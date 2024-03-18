using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public required string Token { get; set; }
    }
}
