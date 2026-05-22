using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MarketplaceApi.Models
{
    public class Mensaje
    {

        public string? ConversacionId { get; set; } // puede ser combinación de remitente-destinatario o generar un hash
        public string MensajeId { get; set; } = ObjectId.GenerateNewId().ToString(); // o cualquier ID único
        public string RemitenteId { get; set; } // ID de MongoDB
        public string DestinatarioId { get; set; } // ID de MongoDB
        public string PublicacionId { get; set; } // ID de MongoDB
        public string Contenido { get; set; }
        public bool Leido { get; set; } = false;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    }
}