using OKeeffeCraft.Core.Interfaces;
using OKeeffeCraft.ExternalServiceProviders.Interfaces;
using OKeeffeCraft.Models;
using OKeeffeCraft.Models.OpenAI;

namespace OKeeffeCraft.Core.Services
{
    public class AMLAssistantService : IAMLAssistantService
    {
        private readonly IChatGptService _chatGptService;
        public AMLAssistantService(IChatGptService chatGptService)
        {
            _chatGptService = chatGptService;
        }

        public async Task<ServiceResponse<object>> ListAssistants()
        {
            return await _chatGptService.ListAssistants();
        }

        public async Task<ServiceResponse<object>> RetrieveAssistant()
        {
            return await _chatGptService.RetrieveAssistant();
        }

        public async Task<ServiceResponse<object>> CreateAssistantThread()
        {
            return await _chatGptService.CreateAssistantThread();
        }

        public async Task<ServiceResponse<object>> CreateAndSendMessage(AssistantMessageRequest model)
        {
            return await _chatGptService.CreateAndSendMessage(model);
        }

        public async Task<ServiceResponse<object>> ListThreadMessages(string threadId)
        {
            return await _chatGptService.ListThreadMessages(threadId);
        }

        public async Task<ServiceResponse<object>> GetAssistantThread(string threadId)
        {
            return await _chatGptService.GetAssistantThread(threadId);
        }

        public async Task<ServiceResponse<object>> RetriveMessage(RetriveMessageRequest model)
        {
            return await _chatGptService.RetriveMessage(model);
        }

        public async Task<ServiceResponse<object>> ListThreadRuns(string threadId)
        {
            return await _chatGptService.ListThreadRuns(threadId);
        }

        public async Task<ServiceResponse<object>> CreateRun(string text)
        {
            return await _chatGptService.CreateRun(text);
        }

        public async Task<ServiceResponse<object>> RetrieveRun(string threadId, string runId)
        {
            return await _chatGptService.RetrieveRun(threadId, runId);
        }

        public async Task<ServiceResponse<object>> CreateRunAndGetResult(CreateRunModel model)
        {
            return await _chatGptService.CreateRunAndGetResult(model);
        }
    }
}
