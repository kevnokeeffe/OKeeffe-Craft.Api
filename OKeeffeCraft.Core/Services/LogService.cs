using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Database;
using OKeeffeCraft.Entities;
using Org.BouncyCastle.Tls;

namespace OKeeffeCraft.Core.Services
{
    public class LogService : ILogService
    {
        private readonly DataContext _context;

        public LogService(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Logs an activity message for a specified account asynchronously.
        /// </summary>
        /// <param name="message">The activity message to log.</param>
        /// <param name="accountId">The ID of the account associated with the activity.</param>
        public async Task ActivityLog(string message, string? identifierType, string? identifier)
        {
            // Add the activity log to the database asynchronously
            await _context.ActivityLogs.AddAsync(new ActivityLog { LogDate = DateTime.UtcNow, LogDetails = message,IdentifierType = identifier, Identifier = identifier });
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Logs an error message with stack trace for a specified account asynchronously.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="stackTrace">The stack trace associated with the error.</param>
        /// <param name="accountId">The ID of the account associated with the error.</param>
        public async Task ErrorLog(string message, string? stackTrace, string? identifierType, string? identifier)
        {
            // Add the error log to the database asynchronously
            await _context.ErrorLogs.AddAsync(new ErrorLog { LogDate = DateTime.UtcNow, LogDetails = message, StackTrace = stackTrace, IdentifierType = identifier, Identifier = identifier });
            await _context.SaveChangesAsync();
        }

    }
}
