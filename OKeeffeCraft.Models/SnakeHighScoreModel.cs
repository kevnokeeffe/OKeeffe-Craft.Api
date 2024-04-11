namespace OKeeffeCraft.Models
{
    public class SnakeHighScoreModel
    {
        public string? Id { get; set; }
        public required string Score { get; set; }
        public required string PlayerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
