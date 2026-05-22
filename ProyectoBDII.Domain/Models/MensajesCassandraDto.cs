using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Models
{
    public class MensajesCassandraDto
    {


        public string? ConversacionId { get; set; }
        public string DestinatarioId { get; set; } // ID de MongoDB
        public string PublicacionId { get; set; } // ID de MongoDB
        public string Contenido { get; set; }


    
 
    }
}
