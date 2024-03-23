namespace OKeeffeCraft.Models.OpenAI
{
    public class CreateRunModel
    {
        public required string Message { get; set; }
        public string? ThreadId { get; set; }
    }
}
