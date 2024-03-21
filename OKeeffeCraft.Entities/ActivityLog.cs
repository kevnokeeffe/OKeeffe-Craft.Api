namespace OKeeffeCraft.Entities
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public DateTime? LogDate { get; set; }
        public string? IdentifierType { get; set; }
        public string? Identifier { get; set; }
        public string? LogDetails { get; set; }

    }
}
