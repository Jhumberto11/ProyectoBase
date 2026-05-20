using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Models
{
    public class ConversacionUsuarioDto
    {
        public string UsuarioId { get; set; } = string.Empty;
        public string ConversacionId { get; set; } = string.Empty;
        public string OtroUsuarioId { get; set; } = string.Empty;
        public string PublicacionId { get; set; } = string.Empty;
        public string UltimoMensaje { get; set; } = string.Empty;
        public DateTime UltimaFecha { get; set; }
    }
}
