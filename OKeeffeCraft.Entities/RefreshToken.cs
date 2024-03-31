using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace OKeeffeCraft.Entities
{
    [Owned]
    public class RefreshToken
    {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Account")]
        public Account? Account { get; set; }

        [BsonElement("Token")]
        public string? Token { get; set; }

        [BsonElement("Expires")]
        public DateTime Expires { get; set; }

        [BsonElement("Created")]
        public DateTime Created { get; set; }

        [BsonElement("CreatedByIp")]
        public string? CreatedByIp { get; set; }

        [BsonElement("Revoked")]
        public DateTime? Revoked { get; set; }

        [BsonElement("RevokedByIp")]
        public string? RevokedByIp { get; set; }

        [BsonElement("ReplacedByToken")]
        public string? ReplacedByToken { get; set; }

        [BsonElement("ReasonRevoked")]
        public string? ReasonRevoked { get; set; }

        [BsonElement("IsExpired")]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [BsonElement("IsRevoked")]
        public bool IsRevoked => Revoked != null;

        [BsonElement("IsActive")]
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
