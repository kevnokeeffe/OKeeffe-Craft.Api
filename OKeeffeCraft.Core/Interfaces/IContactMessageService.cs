using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IContactMessageService
    {
        Task<ServiceResponse<string>> Create(ContactMessage contactMessage);
        Task<ServiceResponse<IEnumerable<ContactMessage>>> Get();
        Task<ServiceResponse<ContactMessage>> GetById(string id);
        Task<ServiceResponse<string>> Update(string id, ContactMessage contactMessage);
        Task<ServiceResponse<string>> Delete(string id);
    }
}
