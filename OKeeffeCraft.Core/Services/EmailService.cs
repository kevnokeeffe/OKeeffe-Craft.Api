using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.Helpers;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Email;
using System.Text;


namespace OKeeffeCraft.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IPostmarkEmailServiceProvider _mailer;
        private readonly IMapper _mapper;
        private readonly IMongoDBService _context;
        private readonly ILogService _logService;
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;


        public EmailService(IPostmarkEmailServiceProvider mailer, IOptions<AppSettings> appSettings, IMapper mapper, IMongoDBService context, ILogService logService, IConfiguration configuration)
        {
            _mailer = mailer;
            _mapper = mapper;
            _context = context;
            _logService = logService;
            _appSettings = appSettings.Value;
            _configuration = configuration; 
        }

        public async Task<ServiceResponse<IEnumerable<EmailModel>>> getEmails()
        {
            var emails = await _context.GetEmailsAsync();
            var response = new ServiceResponse<IEnumerable<EmailModel>>();
            response.Data = _mapper.Map<IEnumerable<EmailModel>>(emails).OrderByDescending(email => email.SentDate);
            response.Message = "Emails retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task<ServiceResponse<EmailModel>> getEmailById(string id)
        {
            var email = await _context.GetEmailAsync(id);
            var response = new ServiceResponse<EmailModel>();
            response.Data = _mapper.Map<EmailModel>(email);
            response.Message = "Email retrieved successfully";
            response.Success = true;
            return response;
        }

        public async Task SendConfirmEmailMessage(ConfirmEmailModel model)
        {
            var subject = "Confirm your email address for KevOKeeffe.ie";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Thanks for registering with KevOKeeffe.ie. Please click on the following link to confirm your email address:");
            sb.AppendLine();
            sb.AppendLine($"{_appSettings.ClientUrl}/verify-email?token={model.AccessToken}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Regards");
            sb.AppendLine("Kevin O'Keeffe");

            NewEmailModel message = new NewEmailModel
            {
                ToName = model.Name,
                ToEmail = model.Email,
                Subject = subject,
                Body = sb.ToString(),
                AccountId = model.AccountId
            };

            await SendMail(message);
        }

        public async Task SendPasswordResetEmail(Account account)
        {
            var subject = "Reset your password for KevOKeeffe.ie";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Please click the below link to reset your password, the link will be valid for 1 day:");
            sb.AppendLine();
            sb.AppendLine($"{_appSettings.ClientUrl}/reset-password?token={account.ResetToken}");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Regards");
            sb.AppendLine("Kevin O'Keeffe");

            NewEmailModel message = new NewEmailModel
            {
                ToName = account.FullName,
                ToEmail = account.Email,
                Subject = subject,
                Body = sb.ToString(),
                AccountId = account.Id
            };

            await SendMail(message);
        }

        public async Task ValidateAndSendMail(NewEmailModel message)
        {

            if (message == null)
                throw new Exception("No email message provided");

            if (message.Body.IsNullOrEmpty() || message.Subject.IsNullOrEmpty() || message.ToEmail.IsNullOrEmpty())
                throw new Exception("ToEmail ,Body and Subject must be provided");

            if (message.ToEmail.IsValidEmail())
                throw new Exception("Invalid ToEmail address provided");

            if (!message.AccountId.IsNullOrEmpty())
            {
                var result = await _context.GetAccountByIdAsync(message.AccountId);
                if (result == null)
                    throw new Exception("Invalid ID provided");
            }
            await SendMail(message);
        }

        public async Task ProcessCallback(string body, string token)
        {
            //check token first
            if (token != _configuration["EmailDeliveryWebhookToken"])
                return;
            try
            {
                //call provider to parse callback
                EmailDeliveryInfo? result = _mailer.ProcessCallback(body);
                //update email record
                if (result != null && result.ExternalRef != null && !result.ExternalRef.IsNullOrEmpty())
                {
                    var emailResult = await _context.GetEmailByExternalRefAsync(result.ExternalRef);
                    if (emailResult != null && emailResult.Id != null)
                    {
                        emailResult.Status = result.Status;
                        emailResult.DeliveryMessage = result.DeliveryMessage;
                        await _context.UpdateEmailAsync(emailResult.Id, emailResult);
                    }
                    // Log the activity
                    await _logService.ActivityLog(ActivityLogTypes.ProcessCallback, ActivityLogTypes.GetDescription(ActivityLogTypes.ProcessCallback) + $", Process callback delivery message: {result.DeliveryMessage}", null);
                }
            }
            catch (Exception ex)
            {
                //Log error
                await _logService.ErrorLog(ErrorLogTypes.ProcessCallback, ex.Message, ex.StackTrace, "Process callback error");
                throw new Exception("POSTMARK PROCESS CALLBACK ERROR: " + ex.Message);
            }
        }


        #region private support methods

        private async Task SendMail(NewEmailModel message)
        {
            var email = new Email
            {
                Body = message.Body,
                EmailDate = DateTime.UtcNow,
                Subject = message.Subject,
                ToEmail = message.ToEmail,
                ToName = message.ToName,
                AccountId = message.AccountId,
                Status = EmailStatuses.Pending,
            };

            try
            {
                //call provider to send email
                var messageId = email.ExternalRef = await _mailer.SendMail(message);
                email.Status = EmailStatuses.Sent;
                email.SentDate = DateTime.UtcNow;

                // Log activity
                await _logService.ActivityLog(ActivityLogTypes.EmailSent, ActivityLogTypes.GetDescription(ActivityLogTypes.EmailSent) + $" Email id: {messageId}, Email subject: {message.Subject}", message.AccountId);

            }
            catch (Exception ex)
            {
                // Log error 
                await _logService.ErrorLog(ErrorLogTypes.SendMail, ex.Message, ex.StackTrace, "Process callback");
                throw new Exception("POSTMARK SEND MAIL ERROR: " + ex.Message);
            }
            finally
            {
                if (_appSettings.ArchiveEmails)
                {
                    await _context.CreateEmailAsync(email);
                }
            }
        }

        #endregion
    }

}
