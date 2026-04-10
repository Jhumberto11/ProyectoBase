using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketplaceApi.Models
{
    public class Publicacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("title")]
        public string Title { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("price")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; } = "USD";

        [BsonElement("condition")]
        public string Condition { get; set; } = "used"; // new, used

        [BsonElement("status")]
        public string Status { get; set; } = "active"; // active, paused, sold, deleted

        [BsonElement("categoryId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; } = null!;

        [BsonElement("sellerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SellerId { get; set; } = null!;

        [BsonElement("locationCity")]
        public string? LocationCity { get; set; }

        [BsonElement("locationDepartment")]
        public string? LocationDepartment { get; set; }


        [BsonElement("images")]
        public List<PublicacionImagenes>? Images { get; set; } = new();

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("attributes")]
        public BsonDocument Attributes { get; set;  } = new BsonDocument();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("pausedAt")]
        public DateTime? PausedAt { get; set; }
    }
}
