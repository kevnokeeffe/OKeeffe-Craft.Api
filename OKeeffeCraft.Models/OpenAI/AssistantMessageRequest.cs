namespace OKeeffeCraft.Models.OpenAI
{
    public class AssistantMessageRequest
    {
        public required string ThreadId { get; set; }
        public required string Message { get; set; }
    }
}
