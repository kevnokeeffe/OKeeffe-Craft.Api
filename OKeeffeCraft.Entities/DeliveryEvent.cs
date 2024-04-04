namespace OKeeffeCraft.Entities
{
    public class DeliveryEvent
    {
        public required string Email { get; set; }
        public required string RecordType { get; set; }
        public required string MessageID { get; set; }
        public required string Description { get; set; }
        public required string Details { get; set; }
    }
}
