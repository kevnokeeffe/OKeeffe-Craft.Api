namespace OKeeffeCraft.Models.OpenAI
{
    public class RetriveMessageRequest
    {
        // public required string AssistantId { get; set; }
        public required string ThreadId { get; set; }
        public required string MessageId { get; set; }
    }
}
