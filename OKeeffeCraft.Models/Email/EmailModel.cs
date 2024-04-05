namespace OKeeffeCraft.Models.Email
{
    public class EmailModel
    {
        public string? Id { get; set; }
        public DateTime EmailDate { get; set; }
        public required string ToEmail { get; set; }
        public required string ToName { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string AccountId { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? LastSentDate { get; set; }
        public string? DeliveryMessage { get; set; }
        public string? ExternalRef { get; set; }
        public string? Status { get; set; }
    }
}
