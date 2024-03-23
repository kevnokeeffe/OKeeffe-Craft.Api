using OKeeffeCraft.Models;
using OKeeffeCraft.Models.OpenAI;

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
        Task<ServiceResponse<object>> RetriveMessage(RetriveMessageRequest model);
        Task<ServiceResponse<object>> ListThreadRuns(string threadId);
        Task<ServiceResponse<object>> CreateRun(string text);
        Task<ServiceResponse<object>> RetrieveRun(string threadId, string runId);
        Task<ServiceResponse<object>> CreateRunAndGetResult(CreateRunModel model);
    }
}
