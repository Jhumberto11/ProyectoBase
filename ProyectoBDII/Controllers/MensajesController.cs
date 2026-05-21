using MarketplaceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Servers;
using ProyectoBDII.Application.Service.CassandraService;
using ProyectoBDII.Domain.Models;
using System.Security.Claims;
using System.Xml;

namespace ProyectoBDII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MensajesController : ControllerBase
    {
        private readonly CassandraMensajeService _service;

        public MensajesController(CassandraMensajeService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje([FromBody] MensajesCassandraDto dto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (dto.ConversacionId == null)
            {
                dto.ConversacionId = $"{user}-{dto.DestinatarioId}-{dto.PublicacionId}";
            }

            var partes = dto.ConversacionId.Split('-');
            if (partes.Length >= 3)
            {
                string user1 = partes[0];
                string user2 = partes[1];
                string publicacion = partes[2];

                // Si el remitente es user1, destinatario es user2; si es user2, destinatario es user1
                dto.DestinatarioId = user == user1 ? user2 : user1;
                dto.PublicacionId = publicacion;
            }

            var mensaje = new Mensaje()
            {

                ConversacionId = dto.ConversacionId,
                RemitenteId = user,
                DestinatarioId = dto.DestinatarioId,
                PublicacionId = dto.PublicacionId,
                Contenido = dto.Contenido,
            };

            await _service.EnviarMensaje(mensaje);

            return Ok(new
            {
                mensaje = "Mensaje enviado correctamente",
                mensajeId = mensaje.MensajeId,
                conversacionId = dto.ConversacionId
            });




        }


        [HttpGet("conversacion/{conversacionId}")]
        public async Task<IActionResult> ObtenerPorConversacion(string conversacionId)
        {
            var mensajes = await _service.ObtenerMensajesPorConversacion(conversacionId);

            return Ok(mensajes);
        }

        [HttpGet("mis-conversaciones")]
        public async Task<IActionResult> ObtenerMisConversaciones()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conversacionIds = await _service.ObtenerConversacionesUnicasPorUsuario(usuarioId);

            return Ok(conversacionIds);
        }




    }
}
