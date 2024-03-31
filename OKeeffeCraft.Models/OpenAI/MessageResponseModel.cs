namespace OKeeffeCraft.Models.OpenAI
{
    public class MessageResponseModel
    {
        public required string FirstId { get; set; }
        public required string LastId { get; set; }
        public List<MessageModel>? Messages { get; set; }
    }

    public class MessageModel
    {
        public string? Id { get; set; }
        public string? ThreadId { get; set; }
        public string? RunId { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
