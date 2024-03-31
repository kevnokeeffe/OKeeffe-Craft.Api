using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IEmailService 
    {
        void SendVerificationEmail(Account account);
        void SendAlreadyRegisteredEmail(string email);
        void SendPasswordResetEmail(Account account);
    }
}
