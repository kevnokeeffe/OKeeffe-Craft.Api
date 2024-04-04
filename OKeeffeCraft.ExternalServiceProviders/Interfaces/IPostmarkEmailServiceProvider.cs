
using OKeeffeCraft.Entities;
using OKeeffeCraft.Models.Email;

namespace OKeeffeCraft.ExternalServiceProviders.Interfaces
{
    public interface IPostmarkEmailServiceProvider
    {
        EmailDeliveryInfo ProcessCallback(string body);
        Task<string> SendMail(NewEmailModel message);
        Task SendTestMail();
    }
}
