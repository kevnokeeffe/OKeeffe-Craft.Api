using OKeeffeCraft.Entities;
using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Models.Accounts
{
    public class CreateRequest
    {

        [Required]
        public required string FullName { get; set; }

        [Required]
        [EnumDataType(typeof(Role))]
        public required string Role { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }
    }
}
