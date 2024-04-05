using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Email;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IEmailService 
    {
        Task SendConfirmEmailMessage(ConfirmEmailModel model);
        Task ValidateAndSendMail(NewEmailModel message);
        Task SendPasswordResetEmail(Account account);
        Task ProcessCallback(string body, string token);
        Task<ServiceResponse<IEnumerable<EmailModel>>> getEmails();
        Task<ServiceResponse<EmailModel>> getEmailById(string id);
    }
}
