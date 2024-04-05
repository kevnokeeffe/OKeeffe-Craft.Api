using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Helpers
{
    public class ErrorLogTypes
    {
        static readonly List<ErrorLogType> _logTypes;

        public const string Login = "login";
        public const string Register = "register";
        public const string VerifyEmail = "verify_email";
        public const string PasswordResetRequest = "password_reset_request";
        public const string PasswordReset = "password_reset";
        public const string ValidateResetToken = "validate_reset_token";
        public const string ProcessCallback = "process_callback";
        public const string SendMail = "send_mail";
        public const string SendMailError = "send_mail_error";
        public const string CreateAccount = "create_account";
        public const string UpdateAccount = "update_account";
        public const string DeleteAccount = "delete_account";

        static ErrorLogTypes()
        {
            _logTypes =
            [
                new ErrorLogType { Code = Login, Name = "Login", Description = "An error has occured while logging in" },
                new ErrorLogType { Code = Register, Name = "Register", Description = "An error has occured while registering an account" },
                new ErrorLogType { Code = VerifyEmail, Name = "Verify Email", Description = "An error has occured while verifying an email" },
                new ErrorLogType { Code = PasswordResetRequest, Name = "Password Reset Request", Description = "An error has occured while resetting a password" },
                new ErrorLogType { Code = PasswordReset, Name = "Password Reset", Description = "An error has occured while resetting a password" },
                new ErrorLogType { Code = ValidateResetToken, Name = "Validate Reset Token", Description = "An error has occured while validating a reset token" },
                new ErrorLogType { Code = ProcessCallback, Name = "Process Callback", Description = "An error has occured during a process callback" },
                new ErrorLogType { Code = SendMail, Name = "Send Mail", Description = "An error has occured during the email sending process" },
                new ErrorLogType { Code = SendMailError, Name = "Send Mail Error", Description = "An error has occured while sending an email" },
                new ErrorLogType { Code = CreateAccount, Name = "Create Account", Description = "An error has occured while creating an account" },
                new ErrorLogType { Code = UpdateAccount, Name = "Update Account", Description = "An error has occured while updating an account" },
                new ErrorLogType { Code = DeleteAccount, Name = "Delete Account", Description = "An error has occured while deleting an account" },
            ];
        }

        public static IList<ErrorLogType> Get()
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
