using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketplaceApi.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("lastName")]
        public string LastName { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("profileImageUrl")]
        public string? ProfileImageUrl { get; set; } 

        [BsonElement("city")]
        public string? City { get; set; }

        [BsonElement("department")]
        public string? Department { get; set; }

        [BsonElement("country")]
        public string? Country { get; set; }

        [BsonElement("role")]
        public string Role { get; set; } = "user";

        [BsonElement("status")]
        public string Status { get; set; } = "active";

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
