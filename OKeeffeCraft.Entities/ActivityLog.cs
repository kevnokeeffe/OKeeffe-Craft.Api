using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OKeeffeCraft.Entities
{
    public class ActivityLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("LogDate")]
        public DateTime? LogDate { get; set; }

        [BsonElement("IdentifierType")]
        public string? IdentifierType { get; set; }

        [BsonElement("Identifier")]
        public string? Identifier { get; set; }

        [BsonElement("LogDetails")]
        public string? LogDetails { get; set; }

    }
}
