using OKeeffeCraft.Entities;
using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Models.Accounts
{
    public class UpdateRequest
    {
        private string? _password;
        private string? _confirmPassword;
        private string? _role;
        private string? _email;

        public string? FullName { get; set; }

        [EnumDataType(typeof(Role))]
        public string? Role
        {
            get => _role;

            set => _role = ReplaceEmptyWithNull(value);
        }

        [EmailAddress]
        public string? Email
        {
            get => _email;
            set => _email = ReplaceEmptyWithNull(value);
        }

        [MinLength(6)]
        public string? Password
        {
            get => _password;

            set => _password = ReplaceEmptyWithNull(value);
        }

        [Compare("Password")]
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set => _confirmPassword = ReplaceEmptyWithNull(value);
        }

        // helpers

        private static string? ReplaceEmptyWithNull(string? value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
