using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IEmailService 
    {
        void SendVerificationEmail(Account account, string origin);
        void SendAlreadyRegisteredEmail(string email, string origin);
        void SendPasswordResetEmail(Account account, string origin);
    }
}
