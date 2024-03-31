using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace OKeeffeCraft.Entities
{
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("FullName")]
        public required string FullName { get; set; }

        [BsonElement("Email")]
        public required string Email { get; set; }

        [BsonElement("PasswordHash")]
        public required string PasswordHash { get; set; }

        [BsonElement("AcceptTerms")]
        public bool AcceptTerms { get; set; }

        [BsonElement("Role")]
        public Role Role { get; set; }

        [BsonElement("VerificationToken")]
        public string? VerificationToken { get; set; }

        [BsonElement("Verified")]
        public DateTime? Verified { get; set; }

        [BsonElement("IsVerified")]
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;

        [BsonElement("ResetToken")]
        public string? ResetToken { get; set; }

        [BsonElement("ResetTokenExpires")]
        public DateTime? ResetTokenExpires { get; set; }

        [BsonElement("PasswordReset")]
        public DateTime? PasswordReset { get; set; }

        [BsonElement("Created")]
        public DateTime? Created { get; set; }

        [BsonElement("Updated")]
        public DateTime? Updated { get; set; }

        [BsonElement("RefreshTokens")]
        public required List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.Find(x => x.Token == token) != null;
        }
    }
}
