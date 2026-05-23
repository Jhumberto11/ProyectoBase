using MarketplaceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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

        [HttpGet("conversacion/{conversacionId}/{limit}")]
        public async Task<IActionResult> ObtenerPorConversacion(string conversacionId, [FromRoute] int limit)
        {
            var mensajes = await _service.ObtenerMensajesPorConversacion(conversacionId, limit);

            return Ok(mensajes);
        }

        [HttpGet("mis-conversaciones/{fecha}")]   
        public async Task<IActionResult> ObtenerMisConversaciones(DateOnly? fecha)
        {
            var fech = fecha;
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var conversacionIds = await _service.ObtenerConversacionesUnicasPorUsuario(usuarioId, fech);

            return Ok(conversacionIds);
        }

        [HttpGet("{mensajeId}")]
        public async Task<IActionResult> ObtenerMensaje(string mensajeId, string conversacionId, [FromQuery] DateTime fechaEnvio)
        {
            var destinatarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var mensaje = await _service.GetMssg(conversacionId, fechaEnvio, mensajeId);
            if (mensaje == null)
            {
                return NotFound(new { mensaje = "Mensaje no encontrado" });
            }

            if (mensaje.RemitenteId != destinatarioId && mensaje.DestinatarioId != destinatarioId)
            {
                return Unauthorized(new { mensaje = "No tienes permiso para ver este mensaje" });
            }

            return Ok(mensaje);
        }

        [HttpPatch("marcar-leido/{mensajeId}")]
        public async Task<IActionResult> MarcarMensajeComoLeído(string mensajeId, [FromQuery] DateTime fechaEnvio, string conversacionId)
        {
            var destinatarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(mensajeId))
            {
                return BadRequest(new { mensaje = "El ID del mensaje es requerido" });
            }

            var mensaje = await _service.GetMssg(conversacionId, fechaEnvio, mensajeId);

            if (mensaje == null)
            {
                return NotFound(new { mensaje = "Mensaje no encontrado" });
            }

            if (mensaje.DestinatarioId != destinatarioId)
            {
                return Unauthorized(new { mensaje = "No tienes permiso para marcar este mensaje como leído" });
            }

            await _service.MarcarAsRead(conversacionId, fechaEnvio, mensajeId);
            return Ok(new { mensaje = "Mensaje marcado como leído correctamente" });
        }

    }
}
