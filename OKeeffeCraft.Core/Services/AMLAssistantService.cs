using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.OpenAI;
using OpenAI.Threads;
using OpenAI;
using Azure;

namespace OKeeffeCraft.Core.Services
{
    public class AMLAssistantService : IAMLAssistantService
    {
        private readonly IChatGptService _chatGptService;
        private readonly ILogService _logService;
        private readonly IMongoDBService _context;
        public AMLAssistantService(IChatGptService chatGptService, ILogService logService, IMongoDBService context)
        {
            _chatGptService = chatGptService;
            _logService = logService;
            _context = context;
        }

        public async Task<ServiceResponse<object>?> ListAssistants()
        {
            return null; //await _chatGptService.ListAssistants();
        }

        public async Task<ServiceResponse<object>?> RetrieveAssistant()
        {
            return null; // _chatGptService.RetrieveAssistant();
        }

        public async Task<ServiceResponse<object>?> CreateAssistantThread()
        {
            return null; // _chatGptService.CreateAssistantThread();
        }

        public async Task<ServiceResponse<object>?> CreateAndSendMessage(AssistantMessageRequest model)
        {
            return null; // _chatGptService.CreateAndSendMessage(model);
        }

        public async Task<ServiceResponse<object>?> ListThreadMessages(string threadId)
        {
            return null; // _chatGptService.ListThreadMessages(threadId);
        }

        public async Task<ServiceResponse<object>?> GetAssistantThread(string threadId)
        {
            return null; // _chatGptService.GetAssistantThread(threadId);
        }

        public async Task<ServiceResponse<object>?> RetriveMessage(string threadId, string messageId)
        {
            return null; // _chatGptService.RetriveMessage(threadId, messageId);
        }

        public async Task<ServiceResponse<object>?> ListThreadRuns(string threadId)
        {
            return null; // _chatGptService.ListThreadRuns(threadId);
        }

        public async Task<ServiceResponse<object>?> CreateRun(string message)
        {
            return null; // _chatGptService.CreateRun(message);
        }

        public async Task<ServiceResponse<object>?> RetrieveRun(string threadId, string runId)
        {
            return null; // _chatGptService.RetrieveRun(threadId, runId);
        }

        public async Task<ServiceResponse<MessageResponseModel>?> CreateRunAndGetResult(CreateRunModel model)
        {
            //if (model == null) return new ServiceResponse<MessageResponseModel> { Data = null, Message = "Model is null", Success = false };
            //try
            //{
            //    var response = await _chatGptService.CreateRunAndGetResult(model);

            //    if (response.Data == null) return new ServiceResponse<MessageResponseModel> { Data = null, Message = response.Message, Success = response.Success };

            //    var messageResponseModel = SanitizeRunResponse(response.Data);

            //    return new ServiceResponse<MessageResponseModel> { Data = messageResponseModel, Message = response.Message, Success = response.Success };
            //}
            //catch (RequestFailedException ex)
            //{
            //    await _logService.ErrorLog(ex.Message, ex.StackTrace, "AML Assistant", "CreateRunAndGetResult");
            //    return new ServiceResponse<MessageResponseModel> { Data = null, Message = ex.Message, Success = false };
            //}
            return null;

        }

        public async Task<ServiceResponse<object>?> GetThreadMessages(string threadId)
        {
            return null; // _chatGptService.GetThreadMessages(threadId);
        }

        private static MessageResponseModel? SanitizeRunResponse(ListResponse<MessageResponse> response)
        {
            //    MessageResponseModel messageResponseModel = new MessageResponseModel
            //    {
            //        LastId = response.LastId,
            //        FirstId = response.FirstId,
            //        Messages = []
            //    };
            //    foreach (var item in response.Items)
            //    {
            //        MessageModel messageModel = new()
            //        {
            //            Id = item.Id,
            //            ThreadId = item.ThreadId,
            //            RunId = item.RunId,
            //            CreatedAt = item.CreatedAt
            //        };
            //        foreach (var message in item.Content)
            //        {
            //            messageModel.Message = message.Text.Value;
            //            messageResponseModel.Messages.Add(messageModel);
            //        }
            //    }
            //    return messageResponseModel;

            return null;
        }
    }
}
