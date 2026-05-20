using Microsoft.AspNetCore.Http;
using ProyectoBDII.Domain.Models;
using ProyectoBDII.Infraestructure;
using System;
using System.Collections.Generic;
using System.Text;
using Cassandra;

namespace ProyectoBDII.Application.Service.CassandraService
{
    public class CassandraMensajeService
    {
        readonly Cassandra.ISession _session;

        public CassandraMensajeService(CassandraContext context)
        {
            _session = context.Session;

        }

        public async Task EnviarMensaje(MensajesCassandraDto dto)
        {
            var queryMensaje = @"
            INSERT INTO mensajes_por_conversacion
            (conversacion_id, fecha_envio, mensaje_id, remitente_id, destinatario_id, publicacion_id, contenido, leido)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

            var stmtMensaje = await _session.PrepareAsync(queryMensaje);

            await _session.ExecuteAsync(stmtMensaje.Bind(
                dto.ConversacionId,
                dto.FechaEnvio,
                dto.MensajeId,
                dto.RemitenteId,
                dto.DestinatarioId,
                dto.PublicacionId,
                dto.Contenido,
                dto.Leido
            ));

            // Actualizar conversaciones por usuario
            await ActualizarConversacionUsuario(dto.RemitenteId, dto.DestinatarioId, dto);
            await ActualizarConversacionUsuario(dto.DestinatarioId, dto.RemitenteId, dto);
        }

        private async Task ActualizarConversacionUsuario(string usuarioId, string otroUsuarioId, MensajesCassandraDto dto)
        {
            var query = @"
            INSERT INTO conversaciones_por_usuario
            (usuario_id, ultima_fecha, conversacion_id, otro_usuario_id, publicacion_id, ultimo_mensaje)
            VALUES (?, ?, ?, ?, ?, ?)";

            var stmt = await _session.PrepareAsync(query);

            await _session.ExecuteAsync(stmt.Bind(
                usuarioId,
                dto.FechaEnvio,
                dto.ConversacionId,
                otroUsuarioId,
                dto.PublicacionId,
                dto.Contenido
            ));
        }



        public async Task<List<string>> ObtenerConversacionesUnicasPorUsuario(string usuarioId)
        {
            var query = @"
        SELECT conversacion_id
        FROM conversaciones_por_usuario
        WHERE usuario_id = ?";

            var stmt = await _session.PrepareAsync(query);
            var result = await _session.ExecuteAsync(stmt.Bind(usuarioId));

            // Selecciona solo los IDs únicos
            return result.Select(row => row.GetValue<string>("conversacion_id"))
                         .Distinct()
                         .ToList();
        }


        public async Task<List<MensajesCassandraDto>> ObtenerMensajesPorConversacion(string conversacionId)
        {
            var query = @"
            SELECT conversacion_id, fecha_envio, mensaje_id, remitente_id, destinatario_id, publicacion_id, contenido, leido
            FROM mensajes_por_conversacion
            WHERE conversacion_id = ?";

            var stmt = await _session.PrepareAsync(query);
            var result = await _session.ExecuteAsync(stmt.Bind(conversacionId));

            return result.Select(row => new MensajesCassandraDto
            {
                ConversacionId = row.GetValue<string>("conversacion_id"),
                FechaEnvio = row.GetValue<DateTime>("fecha_envio"),
                MensajeId = row.GetValue<string>("mensaje_id"),
                RemitenteId = row.GetValue<string>("remitente_id"),
                DestinatarioId = row.GetValue<string>("destinatario_id"),
                PublicacionId = row.GetValue<string>("publicacion_id"),
                Contenido = row.GetValue<string>("contenido"),
                Leido = row.GetValue<bool>("leido")
            }).ToList();
        }
    }
}
