using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.OpenAI;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Threads;
using System.Diagnostics;
using System.Threading;

namespace OKeeffeCraft.ExternalServiceProviders.Services
{
    public class ChatGptService : IChatGptService
    {
        private readonly ILogService _logService;
        private readonly OpenAIClient _api;
        private readonly string _apiKey;
        private readonly string _amlAssistantId;
        private readonly IConfiguration _configuration;

        public ChatGptService(IConfiguration configuration, ILogService logService)
        {
            _configuration = configuration;
            _logService = logService;
            _apiKey = _configuration["OpenAIKey"]!;
            _amlAssistantId = _configuration["AMLAssistantId"]!;
            _api = new OpenAIClient(_apiKey);
        }

        public async Task<ServiceResponse<object>> ListAssistants()
        {
            try
            {
                var assistantsList = await _api.AssistantsEndpoint.ListAssistantsAsync();
                await _logService.ActivityLog("Assistants list retrieved successfully", "Open AI", "ListAssistantsAsync");
                return new ServiceResponse<object>
                {
                    Data = assistantsList,
                    Message = "Assistants list retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "ListAssistantsAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error retrieving assistants list",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> RetrieveAssistant()
        {
            try
            {
                var assistant = await RetriveAssistantAsync();
                await _logService.ActivityLog("Assistant retrieved successfully", "Open AI", "RetrieveAssistantAsync");
                return new ServiceResponse<object>
                {
                    Data = assistant,
                    Message = "Assistant retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "RetrieveAssistantAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error retrieving assistant",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> CreateAssistantThread()
        {
            try
            {
                var thread = await CreateThreadAsync();
                await _logService.ActivityLog("Assistant thread created successfully", "Open AI", "CreateThreadAsync");
                return new ServiceResponse<object>
                {
                    Data = thread,
                    Message = "Assistant thread created successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "CreateThreadAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error creating assistant thread",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> GetAssistantThread(string threadId)
        {
            try
            {
                var thread = await RetrieveThreadAsync(threadId);
                await _logService.ActivityLog("Assistant thread retrieved successfully", "Open AI", "RetrieveThreadAsync");
                return new ServiceResponse<object>
                {
                    Data = thread,
                    Message = "Assistant thread retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "RetrieveThreadAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error retrieving assistant thread",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> ListThreadMessages(string threadId)
        {
            try
            {
                var messageList = await _api.ThreadsEndpoint.ListMessagesAsync(threadId);
                await _logService.ActivityLog("Messages list retrieved successfully", "Open AI", "ListMessagesAsync");
                return new ServiceResponse<object>
                {
                    Data = messageList,
                    Message = "Messages list retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "ListMessagesAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error retrieving messages list",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> CreateAndSendMessage(AssistantMessageRequest model)
        {
            try
            {
                var response = await CreateMessageAsync(model.ThreadId, model.Message);
                await _logService.ActivityLog("Message sent successfully", "Open AI", "CreateMessageAsync");
                return new ServiceResponse<object>
                {
                    Data = response,
                    Message = "Message sent successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "CreateMessageAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error sending message",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> RetriveMessage(RetriveMessageRequest model)
        {
            try
            {
                var message = await _api.ThreadsEndpoint.RetrieveMessageAsync(model.ThreadId, model.MessageId);
                await _logService.ActivityLog("Message retrieved successfully", "Open AI", "RetrieveMessageAsync");
                return new ServiceResponse<object>
                {
                    Data = message,
                    Message = "Message retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "RetriveMessageAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error retriving message",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> ListThreadRuns(string threadId)
        {
            try
            {
                var runList = await _api.ThreadsEndpoint.ListRunsAsync(threadId);
                await _logService.ActivityLog("Runs list retrieved successfully", "Open AI", "ListRunsAsync");
                return new ServiceResponse<object>
                {
                    Data = runList,
                    Message = "Runs list retrieved successfully",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                await _logService.ErrorLog(ex.Message, ex.StackTrace, "Open AI", "ListRunsAsync");
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Error retriving thread runs",
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<object>> CreateRun(string text)
        {
            var assistant = await RetriveAssistantAsync();
            var thread = await CreateThreadAsync();
            var message = await CreateMessageAsync(thread, text);
            var run = await CreateRunAsync(thread, assistant);
            return new ServiceResponse<object>
            {
                Data = run,
                Message = "Run created successfully",
                Success = true
            };
        }

        public async Task<ServiceResponse<object>> RetrieveRun(string threadId, string runId)
        {
            var run = await RetriveRunAsync(threadId, runId);
            var timeout = TimeSpan.FromSeconds(10); // Set your desired timeout duration here
            var stopwatch = Stopwatch.StartNew();
            // Wait for the run to complete
            while (run.Status != RunStatus.Completed)
            {
                if (stopwatch.Elapsed >= timeout)
                {
                    return new ServiceResponse<object>
                    {
                        Data = null,
                        Message = "Run not completed, response timeout",
                        Success = false
                    };
                }
                await Task.Delay(TimeSpan.FromSeconds(2));
                run = await RetriveRunAsync(threadId, runId);

            }

            var response = await run.ListMessagesAsync();
            return new ServiceResponse<object>
            {
                Data = response,
                Message = "Run retrieved successfully",
                Success = true
            };

        }

        public async Task<ServiceResponse<object>> CreateRunAndGetResult(CreateRunModel model)
        {
            var timeout = TimeSpan.FromSeconds(10); // Set your desired timeout duration here
            var stopwatch = Stopwatch.StartNew();
            var assistant = await RetriveAssistantAsync();

            ThreadResponse? thread;
            if (!model.ThreadId.IsNullOrEmpty() && model.ThreadId != null) thread = await RetrieveThreadAsync(model.ThreadId);
            else thread = await CreateThreadAsync();

            await CreateMessageAsync(thread, model.Message);
            var run = await CreateRunAsync(thread, assistant);
            while (run.Status != RunStatus.Completed)
            {
                if (stopwatch.Elapsed >= timeout)
                {
                    return new ServiceResponse<object>
                    {
                        Data = null,
                        Message = "Run not completed, response timeout",
                        Success = false
                    };
                }
                await Task.Delay(TimeSpan.FromSeconds(2));
                run = await RetriveRunAsync(thread.Id, run.Id);
            }
            if(run.Status == RunStatus.Completed)
            {
                return new ServiceResponse<object>
                {
                    Data = await run.ListMessagesAsync(),
                    Message = "Run completed successfully",
                    Success = false
                };
            }
            else
            {
                return new ServiceResponse<object>
                {
                    Data = null,
                    Message = "Run not completed, response timeout",
                    Success = false
                };
            }
        }

        private async Task<RunResponse> CreateRunAsync(ThreadResponse thread, AssistantResponse assistant)
        {
            var run = await thread.CreateRunAsync(assistant);
            return run;
        }

        private async Task<RunResponse> RetriveRunAsync(string threadId, string runId)
        {
            var run = await _api.ThreadsEndpoint.RetrieveRunAsync(threadId, runId);
            return run;
        }

        private async Task<ThreadResponse> CreateThreadAsync()
        {
            var thread = await _api.ThreadsEndpoint.CreateThreadAsync(_amlAssistantId);
            return thread;
        }

        private async Task<AssistantResponse> RetriveAssistantAsync()
        {
            var assistant = await _api.AssistantsEndpoint.RetrieveAssistantAsync(_amlAssistantId);
            return assistant;
        }

        private async Task<MessageResponse> CreateMessageAsync(string threadId, string text)
        {
            var request = new CreateMessageRequest(
                               content: text
                                          );
            var message = await _api.ThreadsEndpoint.CreateMessageAsync(threadId, request);
            return message;
        }

        private async Task<ThreadResponse> RetrieveThreadAsync(string threadId)
        {
            var thread = await _api.ThreadsEndpoint.RetrieveThreadAsync(threadId);
            return thread;
        }
    }

}
