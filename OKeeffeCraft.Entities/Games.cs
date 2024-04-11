using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OKeeffeCraft.Entities
{
    public class Games
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("SnakeHighScore")]
        public required SnakeHighScore SnakeHighScore { get; set; }
    }
}
