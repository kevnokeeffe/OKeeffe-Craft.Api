using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Helpers
{
    public class ActivityLogTypes
    {
        static readonly List<ActivityLogType> _logTypes;

        public const string Register = "register";
        public const string Login = "login";
        public const string VerifyEmail = "verify_email";
        public const string PasswordResetRequest = "password_reset_request";
        public const string PasswordResetSuccess = "password_reset_success";
        public const string ProcessCallback = "process_callback";
        public const string ValidateResetToken = "validate_reset_token";
        public const string EmailSent = "email_sent";
        public const string CreateAccount = "create_account";
        public const string UpdateAccount = "update_account";
        public const string DeleteAccount = "delete_account";

        static ActivityLogTypes()
        {
            _logTypes =
            [
                new ActivityLogType { Code = Register, Name = "Register", Description = "The user has been registered" },
                new ActivityLogType { Code = Login, Name = "Login", Description = "The user has logged in" },
                new ActivityLogType { Code = VerifyEmail, Name = "Verify Email", Description = "The user verified their email" },
                new ActivityLogType { Code = PasswordResetRequest, Name = "Password Reset Request", Description = "The user has requested their password to be reset" },
                new ActivityLogType { Code = PasswordResetSuccess, Name = "Password Reset Success", Description = "The user has successfully reset their password" },
                new ActivityLogType { Code = ProcessCallback, Name = "Process Callback", Description = "An email process callback has been recived" },
                new ActivityLogType { Code = ValidateResetToken, Name = "Validate Reset Token", Description = "A reset token has been validated" },
                new ActivityLogType { Code = EmailSent, Name = "Email Sent", Description = "An email has been successfully sent" },
                new ActivityLogType { Code = CreateAccount, Name = "Create Account", Description = "An account has been created" },
                new ActivityLogType { Code = UpdateAccount, Name = "Update Account", Description = "An account has been updated" },
                new ActivityLogType { Code = DeleteAccount, Name = "Delete Account", Description = "An account has been deleted" },
            ];
          
        }

        public static IList<ActivityLogType> Get()
        {
            return _logTypes;
        }

        public static string? GetDescription(string code)
        {
            var result = _logTypes.FirstOrDefault(p => p.Code == code);
            if (result == null)
                return null;
            else
                return result.Description;
        }

        public static string? GetName(string code)
        {
            var result = _logTypes.FirstOrDefault(p => p.Code == code);
            if (result == null)
                return null;
            else
                return result.Name;
        }
    }
}
