namespace OKeeffeCraft.Core.Interfaces
{
    public interface ILogService
    {
        Task ActivityLog(string message, string? identifierType, string? identifier);
        Task ErrorLog(string message, string? stackTrace, string? identifierType, string? identifier);
    }
}
