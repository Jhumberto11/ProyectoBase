using Microsoft.AspNetCore.Http;
using ProyectoBDII.Domain.Models;
using ProyectoBDII.Infraestructure;
using System;
using System.Collections.Generic;
using System.Text;
using Cassandra;
using ProyectoBDII.Domain.Interface.Cassandra_Interfaces;
using MarketplaceApi.Models;

namespace ProyectoBDII.Application.Service.CassandraService
{
    public class CassandraMensajeService
    {
        private readonly IMessageRepository _session;

        public CassandraMensajeService(IMessageRepository repository)
        {
            _session = repository;

        }

        public async Task EnviarMensaje(Mensaje mensaje)
        {
            // lógica de destinatario automático, creación de ConversacionId, etc.
            await _session.EnviarMensajeAsync(mensaje);
        }


        public async Task<List<string>> ObtenerConversacionesUnicasPorUsuario(string usuarioId)
        {
            return await _session.ObtenerConversacionesUnicasPorUsuarioAsync(usuarioId);
        }


        public async Task<List<Mensaje>> ObtenerMensajesPorConversacion(string conversacionId)
        {
            return await _session.ObtenerMensajesPorConversacionAsync(conversacionId);

        }

        public async Task<MensajesCassandraDto?> ObtenerMensajeAsync(string conversacionId, DateTime fechaEnvio, string mensajeId)
        {
            var query = @"
            SELECT conversacion_id, fecha_envio, mensaje_id, remitente_id, destinatario_id, publicacion_id, contenido, leido
            FROM mensajes_por_conversacion
            WHERE conversacion_id = ? AND fecha_envio = ? AND mensaje_id = ?";

            var stmt = await _session.PrepareAsync(query);
            var result = await _session.ExecuteAsync(stmt.Bind(conversacionId, fechaEnvio, mensajeId));

            var row = result.FirstOrDefault();
            if (row == null)
                return null;

            return new MensajesCassandraDto
            {
                ConversacionId = row.GetValue<string>("conversacion_id"),
                FechaEnvio = row.GetValue<DateTime>("fecha_envio"),
                MensajeId = row.GetValue<string>("mensaje_id"),
                RemitenteId = row.GetValue<string>("remitente_id"),
                DestinatarioId = row.GetValue<string>("destinatario_id"),
                PublicacionId = row.GetValue<string>("publicacion_id"),
                Contenido = row.GetValue<string>("contenido"),
                Leido = row.GetValue<bool>("leido")
            };
        }

        public async Task MarcarMensajeComoLeído (string conversacionId, DateTime fechaEnvio, string mensajeId)
        {
            var query = @"
            UPDATE mensajes_por_conversacion
            SET leido = true
            WHERE conversacion_id = ? AND fecha_envio = ? AND mensaje_id = ?";

            var stmt = await _session.PrepareAsync(query);

            await _session.ExecuteAsync(stmt.Bind(conversacionId, fechaEnvio, mensajeId));
        }

        
    }
}
