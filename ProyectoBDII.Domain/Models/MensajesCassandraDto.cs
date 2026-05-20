using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Models
{
    public class MensajesCassandraDto
    {
        public string ConversacionId { get; set; } = string.Empty; // puede ser combinación de remitente-destinatario o generar un hash
        public string MensajeId { get; set; } = ObjectId.GenerateNewId().ToString(); // o cualquier ID único
        public string RemitenteId { get; set; } = string.Empty; // ID de MongoDB
        public string DestinatarioId { get; set; } = string.Empty; // ID de MongoDB
        public string PublicacionId { get; set; } = string.Empty; // ID de MongoDB
        public string Contenido { get; set; } = string.Empty;
        public bool Leido { get; set; } = false;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    }
}
