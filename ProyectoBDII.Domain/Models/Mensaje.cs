using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketplaceApi.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("listingId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ListingId { get; set; } = null!;

        [BsonElement("senderId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; } = null!;

        [BsonElement("receiverId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReceiverId { get; set; } = null!;

        [BsonElement("messageText")]
        public string MessageText { get; set; } = null!;

        [BsonElement("isRead")]
        public bool IsRead { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}