using OKeeffeCraft.Models;
using OKeeffeCraft.Models.OpenAI;
using OpenAI.Threads;
using OpenAI;

namespace OKeeffeCraft.Core.Interfaces
{
    public interface IAMLAssistantService
    {
        Task<ServiceResponse<object>> ListAssistants();
        Task<ServiceResponse<object>> RetrieveAssistant();
        Task<ServiceResponse<object>> CreateAssistantThread();
        Task<ServiceResponse<object>> CreateAndSendMessage(AssistantMessageRequest model);
        Task<ServiceResponse<object>> ListThreadMessages(string threadId);
        Task<ServiceResponse<object>> GetAssistantThread(string threadId);
        Task<ServiceResponse<object>> RetriveMessage(string threadId, string messageId);
        Task<ServiceResponse<object>> ListThreadRuns(string threadId);
        Task<ServiceResponse<object>> CreateRun(string message);
        Task<ServiceResponse<object>> RetrieveRun(string threadId, string runId);
        Task<ServiceResponse<MessageResponseModel>> CreateRunAndGetResult(CreateRunModel model);
        Task<ServiceResponse<object>> GetThreadMessages(string threadId);
    }
}
