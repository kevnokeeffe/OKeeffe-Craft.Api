using AutoMapper;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Entities;
using OKeeffeCraft.Helpers;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.Logs;

namespace OKeeffeCraft.Core.Services
{
    public class LogService : ILogService
    {
        private readonly IMongoDBService _context;
        private readonly IMapper _mapper;

        public LogService(IMongoDBService context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            await _context.CreateActivityLogAsync(new ActivityLog { LogDate = DateTime.UtcNow, LogDetails = message, IdentifierType = identifierType, Identifier = identifier });
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
            await _context.CreateErrorLogAsync(new ErrorLog { LogDate = DateTime.UtcNow, LogDetails = message, StackTrace = stackTrace, IdentifierType = identifierType, Identifier = identifier });
        }

        /// <summary>
        /// Method to asynchronously retrieve activity logs from the database
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<ActivityLogModel>>> GetActivityLogs()
        {
            try
            {
                // Retrieve activity logs from the database asynchronously
                var logs = await _context.GetActivityLogsAsync();

                // If no logs are found, throw an exception
                if (logs == null || logs.Count == 0)
                    throw new AppException("No activity logs found");

                var activityLogs = _mapper.Map<IEnumerable<ActivityLogModel>>(logs);

                // Return a successful response with the retrieved logs
                return new ServiceResponse<IEnumerable<ActivityLogModel>> { Success = true, Message = "Success, activity logs found", Data = activityLogs };
            }
            catch (Exception ex)
            {
                await ErrorLog(ex.Message, ex.StackTrace, "LogService", "GetActivityLogs");
                return new ServiceResponse<IEnumerable<ActivityLogModel>> { Success = false, Message = ex.Message, Data = null };

            }
        }

        /// <summary>
        /// Method to asynchronously retrieve error logs from the database
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<ErrorLogModel>>> GetErrorLogs()
        {
            try {
            // Retrieve error logs from the database asynchronously
            var logs = await _context.GetErrorLogsAsync();

            // If no logs are found, throw an exception
            if (logs == null || logs.Count == 0)
                throw new AppException("No error logs found");

            var errorLogs = _mapper.Map<IEnumerable<ErrorLogModel>>(logs);

            // Return a successful response with the retrieved logs
            return new ServiceResponse<IEnumerable<ErrorLogModel>> { Success = true, Message = "Success, error logs found", Data = errorLogs };
                }
            catch (Exception ex)
            {
                await ErrorLog(ex.Message, ex.StackTrace, "LogService", "GetErrorLogs");
                return new ServiceResponse<IEnumerable<ErrorLogModel>> { Success = false, Message = ex.Message, Data = null };
            }
        }

        /// <summary>
        /// Method to asynchronously retrieve an activity log by its ID
        /// </summary>
        /// <param name="id">The ID of the activity log to retrieve</param>
        public async Task<ServiceResponse<ActivityLogModel>> GetActivityLogById(string id)
        {
            try
            {
                // Retrieve the activity log from the database asynchronously
                var activityLog = await _context.GetActivityLogAsync(id);

                // If no activity log is found, throw an exception
                if (activityLog == null)
                    throw new AppException("No activity log found");

                // Return a successful response with the retrieved activity log
                return new ServiceResponse<ActivityLogModel>
                {
                    Success = true,
                    Message = "Success, activity log found",
                    Data = _mapper.Map<ActivityLogModel>(activityLog)
                };
            }
            catch (Exception ex)
            {
                // Log the error
                await ErrorLog(ex.Message, ex.StackTrace, "LogService", "GetActivityLogById");

                // Return a response indicating failure along with the error message
                return new ServiceResponse<ActivityLogModel>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        /// <summary>
        /// Method to asynchronously retrieve an error log by its ID
        /// </summary>
        /// <param name="id">The ID of the error log to retrieve</param>
        public async Task<ServiceResponse<ErrorLogModel>> GetErrorLogById(string id)
        {
            try
            {
                // Retrieve the error log from the database asynchronously
                var errorLog = await _context.GetErrorLogAsync(id);

                // If no error log is found, throw an exception
                if (errorLog == null)
                    throw new AppException("No error log found");



                // Return a successful response with the retrieved error log
                return new ServiceResponse<ErrorLogModel>
                {
                    Success = true,
                    Message = "Success, error log found",
                    Data = _mapper.Map<ErrorLogModel>(errorLog)
                };
            }
            catch (Exception ex)
            {
                // Log the error
                await ErrorLog(ex.Message, ex.StackTrace, "LogService", "GetErrorLogById");

                // Return a response indicating failure along with the error message
                return new ServiceResponse<ErrorLogModel>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }
    }
}
