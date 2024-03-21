using OKeeffeCraft.Entities;
using OKeeffeCraft.Models;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface ILogService
    {
        Task ActivityLog(string message, string? identifierType, string? identifier);
        Task ErrorLog(string message, string? stackTrace, string? identifierType, string? identifier);
        Task<ServiceResponse<IEnumerable<ActivityLog>>> GetActivityLogs();
        Task<ServiceResponse<IEnumerable<ErrorLog>>> GetErrorLogs();
        Task<ServiceResponse<ActivityLog>> GetActivityLogById(int id);
        Task<ServiceResponse<ErrorLog>> GetErrorLogById(int id);
    }
}
