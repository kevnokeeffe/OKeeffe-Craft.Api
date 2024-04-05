using OKeeffeCraft.Entities;
using OKeeffeCraft.Models.Email;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IEmailService 
    {
        Task SendConfirmEmailMessage(ConfirmEmailModel model);
        Task ValidateAndSendMail(NewEmailModel message);
        Task SendPasswordResetEmail(Account account);
        Task ProcessCallback(string body, string token);
    }
}
