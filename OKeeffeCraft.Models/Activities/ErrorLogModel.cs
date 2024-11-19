namespace OKeeffeCraft.Models.Logs
{
    public class ErrorLogModel
    {
        public string? Id { get; set; }

        public DateTime? LogDate { get; set; }

        public string? IdentifierType { get; set; }

        public string? Identifier { get; set; }

        public string? LogDetails { get; set; }

        public string? StackTrace { get; set; }
    }
}
