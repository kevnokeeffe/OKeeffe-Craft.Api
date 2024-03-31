using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IMongoDBService
    {
        Task<List<Account>> GetAccountsAsync();
        Task<Account> GetAccountByRefreshTokenAsync(string refreshToken);
        Task<Account> GetAccountByResetTokenAsync(string resetToken);
        Task<Account> GetAccountByVerificationTokenAsync(string verificationToken);
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByIdAsync(string id);
        Task CreateAccountAsync(Account newAccount);
        Task UpdateAccountAsync(string id, Account updatedAccount);
        Task RemoveAccountAsync(string id);
        Task<List<ActivityLog>> GetActivityLogsAsync();
        Task<ActivityLog?> GetActivityLogAsync(string id);
        Task CreateActivityLogAsync(ActivityLog newActivityLog);

        Task<List<ErrorLog>> GetErrorLogsAsync();
        Task<ErrorLog?> GetErrorLogAsync(string id);
        Task CreateErrorLogAsync(ErrorLog newErrorLog);
        Task UpdateErrorLogAsync(string id, ErrorLog updatedErrorLog);
        Task RemoveErrorLogAsync(string id);
        Task UpdateActivityLogAsync(string id, ActivityLog updatedActivityLog);
        Task RemoveActivityLogAsync(string id);
    }
}
