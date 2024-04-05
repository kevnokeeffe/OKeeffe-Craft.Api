using MongoDB.Driver;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Database;
using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Core.Services
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoCollection<Account> _accountCollection;
        private readonly IMongoCollection<ActivityLog> _activityLogCollection;
        private readonly IMongoCollection<ErrorLog> _errorLogCollection;
        private readonly IMongoCollection<RefreshToken> _refreshTokenCollection;
        private readonly IMongoCollection<Email> _emailCollection;

        private readonly MongoDataContext _context;

        public MongoDBService(MongoDataContext context)
        {
            _context = context;
            _accountCollection = _context.db.GetCollection<Account>("Accounts");
            _activityLogCollection = _context.db.GetCollection<ActivityLog>("ActivityLogs");
            _errorLogCollection = _context.db.GetCollection<ErrorLog>("ErrorLogs");
            _refreshTokenCollection = _context.db.GetCollection<RefreshToken>("RefreshTokens");
            _emailCollection = _context.db.GetCollection<Email>("Email");
        }

        #region Email
        public async Task<List<Email>> GetEmailsAsync() =>
            await _emailCollection.Find(_ => true).ToListAsync();

        public async Task<Email?> GetEmailAsync(string id) =>
            await _emailCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateEmailAsync(Email newEmail) =>
            await _emailCollection.InsertOneAsync(newEmail);

        public async Task UpdateEmailAsync(string id, Email updatedEmail) =>
            await _emailCollection.ReplaceOneAsync(x => x.Id == id, updatedEmail);

        public async Task RemoveEmailAsync(string id) =>
            await _emailCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Email>> GetEmailsByAccountIdAsync(string accountId) =>
            await _emailCollection.Find(x => x.AccountId == accountId).ToListAsync();

        public async Task<List<Email>> GetEmailsByExternalRefAsync(string externalRef) =>
            await _emailCollection.Find(x => x.ExternalRef == externalRef).ToListAsync();

        public async Task<Email> GetEmailByExternalRefAsync(string externalRef) =>
          await _emailCollection.Find(x => x.ExternalRef == externalRef).FirstOrDefaultAsync();

        public async Task<List<Email>> GetEmailsByStatusAsync(string status) =>
            await _emailCollection.Find(x => x.Status == status).ToListAsync();
        #endregion

        #region Account

        public async Task<List<Account>> GetAccountsAsync() =>
        await _accountCollection.Find(_ => true).ToListAsync();

        public async Task<Account> GetAccountByRefreshTokenAsync(string refreshToken)
        {
            var account =  await _accountCollection.Find(u => u.RefreshTokens.Any(t => t.Token == refreshToken)).FirstOrDefaultAsync();
            return account;
        }
            

        public async Task<Account> GetAccountByResetTokenAsync(string resetToken) =>
            await _accountCollection.Find(x => x.ResetToken == resetToken && x.ResetTokenExpires > DateTime.UtcNow).FirstOrDefaultAsync();

        public async Task<Account> GetAccountByVerificationTokenAsync(string verificationToken) 
            {
                var account = await _accountCollection.Find(x => x.VerificationToken == verificationToken).FirstOrDefaultAsync();
                return account;
            }


        public async Task<Account?> GetAccountByEmailAsync(string email) =>
            await _accountCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task<Account?> GetAccountByIdAsync(string id) =>
            await _accountCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAccountAsync(Account newAccount) =>
            await _accountCollection.InsertOneAsync(newAccount);

        public async Task UpdateAccountAsync(string id, Account updatedAccount) =>
            await _accountCollection.ReplaceOneAsync(x => x.Id == id, updatedAccount);

        public async Task RemoveAccountAsync(string id) =>
            await _accountCollection.DeleteOneAsync(x => x.Id == id);
        #endregion

        #region ActivityLog

        public async Task<List<ActivityLog>> GetActivityLogsAsync() =>
            await _activityLogCollection.Find(_ => true).ToListAsync();

        public async Task<ActivityLog?> GetActivityLogAsync(string id) =>
            await _activityLogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateActivityLogAsync(ActivityLog newActivityLog) =>
            await _activityLogCollection.InsertOneAsync(newActivityLog);

        public async Task UpdateActivityLogAsync(string id, ActivityLog updatedActivityLog) =>
            await _activityLogCollection.ReplaceOneAsync(x => x.Id == id, updatedActivityLog);

        public async Task RemoveActivityLogAsync(string id) =>
            await _activityLogCollection.DeleteOneAsync(x => x.Id == id);
        #endregion

        #region ErrorLog
        public async Task<List<ErrorLog>> GetErrorLogsAsync() =>
            await _errorLogCollection.Find(_ => true).ToListAsync();

        public async Task<ErrorLog?> GetErrorLogAsync(string id) =>
            await _errorLogCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateErrorLogAsync(ErrorLog newErrorLog) =>
            await _errorLogCollection.InsertOneAsync(newErrorLog);

        public async Task UpdateErrorLogAsync(string id, ErrorLog updatedErrorLog) =>
            await _errorLogCollection.ReplaceOneAsync(x => x.Id == id, updatedErrorLog);

        public async Task RemoveErrorLogAsync(string id) =>
            await _errorLogCollection.DeleteOneAsync(x => x.Id == id);
        #endregion

        #region RefreshToken

        public async Task<List<RefreshToken>> GetRefreshTokensAsync() =>
            await _refreshTokenCollection.Find(_ => true).ToListAsync();

        public async Task<RefreshToken?> GetRefreshTokenAsync(string id) =>
            await _refreshTokenCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateRefreshTokenAsync(RefreshToken newRefreshToken) =>
            await _refreshTokenCollection.InsertOneAsync(newRefreshToken);

        public async Task UpdateRefreshTokenAsync(string id, RefreshToken updatedRefreshToken) =>
            await _refreshTokenCollection.ReplaceOneAsync(x => x.Id == id, updatedRefreshToken);

        public async Task RemoveRefreshTokenAsync(string id) =>
            await _refreshTokenCollection.DeleteOneAsync(x => x.Id == id);

        public
            async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token) =>
            await _refreshTokenCollection.Find(x => x.Token == token).FirstOrDefaultAsync();

        public async Task RemoveRefreshTokenByTokenAsync(string token) =>
            await _refreshTokenCollection.DeleteOneAsync(x => x.Token == token);

        #endregion

    }
}
