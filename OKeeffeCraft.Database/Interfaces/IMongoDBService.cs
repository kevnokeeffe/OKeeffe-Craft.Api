using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IMongoDBService
    {
        Task<List<Games>> GetGamesAsync();
        Task<Games?> GetGamesAsync(string id);
        Task CreateGamesAsync(Games newGames);
        Task<List<SnakeHighScore>> GetSnakeHighScoresAsync();
        Task<SnakeHighScore> GetSnakeHighScoreAsync(string id);
        Task CreateSnakeHighScoreAsync(SnakeHighScore newSnakeHighScore);
        Task UpdateSnakeHighScoreAsync(string id, SnakeHighScore updatedSnakeHighScore);
        Task RemoveSnakeHighScoreAsync(string id);
        Task<List<ContactMessage>> GetContactMessagesAsync();
        Task<ContactMessage?> GetContactMessageAsync(string id);
        Task CreateContactMessageAsync(ContactMessage newContactMessage);
        Task UpdateContactMessageAsync(string id, ContactMessage updatedContactMessage);
        Task RemoveContactMessageAsync(string id);
        Task<List<Email>> GetEmailsAsync();
        Task<Email?> GetEmailAsync(string id);
        Task CreateEmailAsync(Email newEmail);
        Task UpdateEmailAsync(string id, Email updatedEmail);
        Task RemoveEmailAsync(string id);
        Task<List<Email>> GetEmailsByAccountIdAsync(string accountId);
        Task<List<Email>> GetEmailsByExternalRefAsync(string externalRef);
        Task<Email> GetEmailByExternalRefAsync(string externalRef);
        Task<List<Email>> GetEmailsByStatusAsync(string status);
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
