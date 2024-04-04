using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Entities
{
    public class Email
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("EmailDate")]
        public DateTime EmailDate { get; set; }

        [Required]
        [MaxLength(1000)]
        [BsonElement("ToEmail")]
        public required string ToEmail { get; set; }

        [MaxLength(1000)]
        [BsonElement("ToName")]
        public required string ToName { get; set; }

        [Required]
        [MaxLength(1000)]
        [BsonElement("Subject")]
        public required string Subject { get; set; }

        [Required]
        [BsonElement("Body")]
        public required string Body { get; set; }

        [MaxLength(40)]
        [BsonElement("Username")]
        public required string Username { get; set; }

        [BsonElement("SentDate")]
        public DateTime? SentDate { get; set; }

        [BsonElement("LastSentDate")]
        public DateTime? LastSentDate { get; set; }

        [BsonElement("DeliveryMessage")]
        public required string DeliveryMessage { get; set; }

        [BsonElement("ExternalRef")]
        public required string ExternalRef { get; set; }

        [BsonElement("Status")]
        public string? Status { get; set; }

    }
}
