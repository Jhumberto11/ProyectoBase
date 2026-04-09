using MongoDB.Bson.Serialization.Attributes;

namespace MarketplaceApi.Models
{
    public class PublicacionImagenes
    {
        [BsonElement("url")]
        public string Url { get; set; } = null!;

        [BsonElement("filename")]
        public string? FileName { get; set; }

        [BsonElement("isMain")]
        public bool IsMain { get; set; } = false;
    }
}
