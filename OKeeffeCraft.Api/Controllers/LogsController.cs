using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Models;
using Swashbuckle.AspNetCore.Annotations;
using OKeeffeCraft.Models.Logs;

namespace OKeeffeCraft.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LogsController : BaseController
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet("error")]
        [SwaggerOperation(Summary = "Gets error logs from the system")]
        [SwaggerResponse(200, "Error logs retrieved.", typeof(ServiceResponse<IEnumerable<ErrorLogModel>>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> GetErrorLogs()
        {
            return Ok(await _logService.GetErrorLogs());
        }

        [HttpGet("activity")]
        [SwaggerOperation(Summary = "Gets activity logs from the system")]
        [SwaggerResponse(200, "Activity logs retrieved.", typeof(ServiceResponse<IEnumerable<ActivityLogModel>>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> GetActivityLogs()
        {
            var logs = await _logService.GetActivityLogs();
            return Ok(logs);
        }

        [HttpGet("error/{id}")]
        [SwaggerOperation(Summary = "Gets a specific error log from the system")]
        [SwaggerResponse(200, "Error log retrieved.", typeof(ServiceResponse<ErrorLogModel>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]
        public async Task<IActionResult> GetErrorLog(string id)
        {
            return Ok(await _logService.GetErrorLogById(id));
        }

        [HttpGet("activity/{id}")]
        [SwaggerOperation(Summary = "Gets a specific activity log from the system")]
        [SwaggerResponse(200, "Activity log retrieved.", typeof(ServiceResponse<ActivityLogModel>))]
        [SwaggerResponse(400, "Error message (generally a validation error)", typeof(ServiceResponse<string>))]

        public async Task<IActionResult> GetActivityLog(string id)
        {
            return Ok(await _logService.GetActivityLogById(id));
        }
    }
}
