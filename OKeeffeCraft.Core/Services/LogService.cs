using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Database;
using OKeeffeCraft.Entities;

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
        /// <param name="identifierType">The type of identifier associated with the activity.</param>
        /// <param name="identifier">The value associated with the identifier type.</param>
        public async Task ActivityLog(string message, string? identifierType = null, string? identifier = null)
        {
            // Add the activity log to the database asynchronously
            await _context.ActivityLogs.AddAsync(new ActivityLog { LogDate = DateTime.UtcNow, LogDetails = message, IdentifierType = identifierType, Identifier = identifier });
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Logs an error message with stack trace for a specified account asynchronously.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        /// <param name="stackTrace">The stack trace associated with the error.</param>
        /// <param name="identifierType">The type of identifier associated with the error.</param>
        /// <param name="identifier">The value associated with the identifier type.</param>
        public async Task ErrorLog(string message, string? stackTrace = null, string? identifierType = null, string? identifier = null)
        {
            // Add the error log to the database asynchronously
            await _context.ErrorLogs.AddAsync(new ErrorLog { LogDate = DateTime.UtcNow, LogDetails = message, StackTrace = stackTrace, IdentifierType = identifierType, Identifier = identifier });
            await _context.SaveChangesAsync();
        }

    }
}
