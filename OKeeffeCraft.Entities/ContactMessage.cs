using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OKeeffeCraft.Entities
{
    public class ContactMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        public required string Email { get; set; }

        [BsonElement("Subject")]
        public required string Subject { get; set; }

        [BsonElement("Message")]
        public required string Message { get; set; }

        [BsonElement("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [BsonElement("IsRead")]
        public bool? IsRead { get; set; }

        [BsonElement("UpdatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }
}
