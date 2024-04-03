using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Logs;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface ILogService
    {
        Task ActivityLog(string message, string? identifierType, string? identifier);
        Task ErrorLog(string message, string? stackTrace, string? identifierType, string? identifier);
        Task<ServiceResponse<IEnumerable<ActivityLogModel>>> GetActivityLogs();
        Task<ServiceResponse<IEnumerable<ErrorLogModel>>> GetErrorLogs();
        Task<ServiceResponse<ActivityLogModel>> GetActivityLogById(string id);
        Task<ServiceResponse<ErrorLogModel>> GetErrorLogById(string id);
    }
}
