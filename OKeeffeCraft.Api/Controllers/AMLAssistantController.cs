using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.OpenAI;
using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Api.Controllers
{
    [Authorize(Entities.Role.Admin)]
    [ApiController]
    [Route("[controller]")]
    public class AMLAssistantController : ControllerBase
    {
        private readonly IAMLAssistantService _amlAssistantService;

        public AMLAssistantController(IAMLAssistantService amlAssistantService)
        {
            _amlAssistantService = amlAssistantService;
        }

        //[HttpGet("assistants")]
        //[SwaggerOperation(Summary = "List Assistants", Description = "List all the assistants")]
        //[SwaggerResponse(200, "Assistants list retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> ListAssistants()
        //{
        //    var response = await _amlAssistantService.ListAssistants();
        //    return Ok(response);
        //}

        //[HttpGet("assistant")]
        //[SwaggerOperation(Summary = "Retrieve Assistant", Description = "Retrieve AML assistant")]
        //[SwaggerResponse(200, "Assistant retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> RetrieveAssistant()
        //{
        //    var response = await _amlAssistantService.RetrieveAssistant();
        //    return Ok(response);
        //}

        //[HttpPost("create-thread")]
        //[SwaggerOperation(Summary = "Create Assistant Thread", Description = "Create a new assistant thread")]
        //[SwaggerResponse(200, "Assistant thread created successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> CreateAssistantThread()
        //{
        //    var response = await _amlAssistantService.CreateAssistantThread();
        //    return Ok(response);
        //}

        //[HttpPost("create-message")]
        //[SwaggerOperation(Summary = "Create and Send Message", Description = "Create and send a message in a thread")]
        //[SwaggerResponse(200, "Message sent successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> CreateAndSendMessageAsync([FromBody] AssistantMessageRequest model)
        //{
        //    var response = await _amlAssistantService.CreateAndSendMessage(model);
        //    return Ok(response);
        //}

        //[HttpGet("thread-messages-details")]
        //[SwaggerOperation(Summary = "List Thread Messages", Description = "List all the messages in a thread")]
        //[SwaggerResponse(200, "Messages list retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> ListThreadMessages(string threadId)
        //{
        //    var response = await _amlAssistantService.ListThreadMessages(threadId);
        //    return Ok(response);
        //}

        //[HttpGet("thread")]
        //[SwaggerOperation(Summary = "Get Assistant Thread", Description = "Retrieve a specific assistant thread")]
        //[SwaggerResponse(200, "Assistant thread retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> GetAssistantThread(string threadId)
        //{
        //    var response = await _amlAssistantService.GetAssistantThread(threadId);
        //    return Ok(response);
        //}

        //[HttpGet("retrieve-message")]
        //[SwaggerOperation(Summary = "Retrieve Message", Description = "Retrieve a specific message")]
        //[SwaggerResponse(200, "Message retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> RetriveMessageAsync(string threadId, string messageId)
        //{
        //    var response = await _amlAssistantService.RetriveMessage(threadId,messageId);
        //    return Ok(response);
        //}

        //[HttpGet("thread-runs")]
        //[SwaggerOperation(Summary = "List Thread Runs", Description = "List all the runs in a thread")]
        //[SwaggerResponse(200, "Runs list retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> ListThreadRuns(string threadId)
        //{
        //    var response = await _amlAssistantService.ListThreadRuns(threadId);
        //    return Ok(response);
        //}

        //[HttpPost("create-run")]
        //[SwaggerOperation(Summary = "Create Run", Description = "Create a new run")]
        //[SwaggerResponse(200, "Run created successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> CreateRun(string message)
        //{
        //    var response = await _amlAssistantService.CreateRun(message);
        //    return Ok(response);
        //}

        //[HttpGet("retrieve-run")]
        //[SwaggerOperation(Summary = "Retrieve Run", Description = "Retrieve a specific run")]
        //[SwaggerResponse(200, "Run retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> RetriveRun(string threadId, string runId)
        //{
        //    var response = await _amlAssistantService.RetrieveRun(threadId, runId);
        //    return Ok(response);
        //}

        //[HttpPost("create-run-result")]
        //[SwaggerOperation(Summary = "Create Run and Get Result", Description = "Create a new run and get the result")]
        //[SwaggerResponse(200, "Run created and result retrieved successfully", typeof(ServiceResponse<MessageResponseModel>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> CreateRunAndGetResult([FromBody] CreateRunModel model)
        //{
        //    var response = await _amlAssistantService.CreateRunAndGetResult(model);
        //    return Ok(response);
        //}

        //[HttpGet("thread-messages")]
        //[SwaggerOperation(Summary = "Get Thread Messages", Description = "Get all the messages in a thread")]
        //[SwaggerResponse(200, "Messages list retrieved successfully", typeof(ServiceResponse<object>))]
        //[SwaggerResponse(500, "Internal server error", typeof(ServiceResponse<string>))]
        //public async Task<IActionResult> GetThreadMessages(string threadId)
        //{
        //    var response = await _amlAssistantService.GetThreadMessages(threadId);
        //    return Ok(response);
        //}
    }
}
