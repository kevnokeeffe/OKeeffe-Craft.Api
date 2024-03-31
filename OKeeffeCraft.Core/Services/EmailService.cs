using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using OKeeffeCraft.Authorization.Interfaces;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Helpers;


namespace OKeeffeCraft.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;
        private readonly ILogService _logService;
        private readonly IAuthIdentityService _authIdentityService;

        public EmailService(IOptions<AppSettings> appSettings, ILogService logService,
            IAuthIdentityService identityService)
        {
            _appSettings = appSettings.Value; _logService = logService;
            _authIdentityService = identityService;
        }

        /// <summary>
        /// Sends a verification email to the specified account for email address verification.
        /// </summary>
        /// <param name="account">The account for which the verification email is being sent.</param>
        /// <param name="origin">The origin URL for the verification link (optional).</param>
        public async void SendVerificationEmail(Account account)
        {
            try
            {
                string message;

                    // Origin exists if the request is sent from a browser single page app (e.g., Angular or React)
                    // So send a link to verify via the single page app
                    var verifyUrl = $"{_appSettings.ClientUrl}/verify-email?token={account.VerificationToken}";
                    message = $@"<p>Please click the below link to verify your email address:</p>
                    <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";

                Send(
                    to: account.Email,
                    subject: "Sign-up Verification API - Verify Email",
                    html: $@"<h4>Verify Email</h4>
                <p>Thanks for registering!</p>
                
                {message}"
                );
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", account.Id.ToString());
            }
        }

        /// <summary>
        /// Sends an email informing the user that the provided email is already registered.
        /// </summary>
        /// <param name="email">The email address that is already registered.</param>
        /// <param name="origin">The origin URL for additional actions (optional).</param>
        public async void SendAlreadyRegisteredEmail(string email)
        {
            try
            {
                string message;

                    message = $@"<p>If you don't know your password please visit the <a href=""{_appSettings.ClientUrl}/account/forgot-password"">forgot password</a> page.</p>";
               

                Send(
                    to: email,
                    subject: "Sign-up Verification API - Email Already Registered",
                    html: $@"<h4>Email Already Registered</h4>
                <p>Your email <strong>{email}</strong> is already registered.</p>
                {message}"
                );
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace,"Email" ,email);
            }
        }

        /// <summary>
        /// Sends a password reset email to the specified account.
        /// </summary>
        /// <param name="account">The account for which the password reset email is being sent.</param>
        /// <param name="origin">The origin URL for the password reset link (optional).</param>
        public async void SendPasswordResetEmail(Account account)
        {
            try
            {
                string message;

                    var resetUrl = $"{_appSettings.ClientUrl}/reset-password?token={account.ResetToken}";
                    message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                    <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
                
              

                Send(
                    to: account.Email,
                    subject: "Sign-up Verification API - Reset Password",
                    html: $@"<h4>Reset Password Email</h4>
                {message}"
                );
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", account.Id.ToString());
            }
        }

        /// <summary>
        /// Sends an email using the specified parameters.
        /// </summary>
        /// <param name="to">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="html">The HTML content of the email.</param>
        /// <param name="from">The email address of the sender (optional). If not provided, the default sender from the application settings is used.</param>
        private async void Send(string to, string subject, string html, string? from = null)
        {
            try
            {
                // create message
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(from ?? _appSettings.EmailFrom));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = html };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
                smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception error)
            {
                await _logService.ErrorLog(error.Message, error.StackTrace, "Account id", _authIdentityService.GetAccountId());
            }
        }
    }
}
