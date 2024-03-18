
using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public required string Token { get; set; }
    }
}
