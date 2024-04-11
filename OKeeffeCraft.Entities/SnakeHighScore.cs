using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OKeeffeCraft.Entities
{
    public class SnakeHighScore
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Score")]
        public required string Score { get; set; }

        [BsonElement("PlayerName")]
        public required string PlayerName { get; set; }

        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [BsonElement("UpdatedDate")]
        public DateTime UpdatedDate { get; set; }
    }
}
