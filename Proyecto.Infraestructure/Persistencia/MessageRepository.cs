using Cassandra;
using MarketplaceApi.Models;
using ProyectoBDII.Domain.Interface.Cassandra_Interfaces;
using ProyectoBDII.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure.Persistencia
{
    public class MessageRepository : IMessageRepository
    {

        private readonly ISession _session;

        public MessageRepository(CassandraContext context)
        {
            _session = context.Session;
        }

        public async Task ActualizarConversacionUsuario(string usuarioId, string otroUsuarioId, Mensaje mensaje)
        {
            var query = @"
            INSERT INTO conversaciones_por_usuario
            (usuario_id, ultima_fecha, conversacion_id, otro_usuario_id, publicacion_id, ultimo_mensaje)
            VALUES (?, ?, ?, ?, ?, ?)";

            var stmt = await _session.PrepareAsync(query);

            await _session.ExecuteAsync(stmt.Bind(
                usuarioId,
                mensaje.FechaEnvio,
                mensaje.ConversacionId,
                otroUsuarioId,
                mensaje.PublicacionId,
                mensaje.Contenido
            ));
        }

        public async Task EnviarMensajeAsync(Mensaje mensaje)
        {
            var queryMensaje = @"
            INSERT INTO mensajes_por_conversacion
            (conversacion_id, fecha_envio, mensaje_id, remitente_id, destinatario_id, publicacion_id, contenido, leido)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?)";

            var stmtMensaje = await _session.PrepareAsync(queryMensaje);

            await _session.ExecuteAsync(stmtMensaje.Bind(
                mensaje.ConversacionId,
                mensaje.FechaEnvio,
                mensaje.MensajeId,
                mensaje.RemitenteId,
                mensaje.DestinatarioId,
                mensaje.PublicacionId,
                mensaje.Contenido,
                mensaje.Leido
            ));

            // Actualizar conversaciones por usuario
            await ActualizarConversacionUsuario(mensaje.RemitenteId, mensaje.DestinatarioId, mensaje);
            await ActualizarConversacionUsuario(mensaje.DestinatarioId, mensaje.RemitenteId, mensaje);
        }

        public async Task<List<string>> ObtenerConversacionesUnicasPorUsuarioAsync(string usuarioId)
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

        public async Task<List<Mensaje>> ObtenerMensajesPorConversacionAsync(string conversacionId)
        {
            var query = @"
            SELECT conversacion_id, fecha_envio, mensaje_id, remitente_id, destinatario_id, publicacion_id, contenido, leido
            FROM mensajes_por_conversacion
            WHERE conversacion_id = ?";

            var stmt = await _session.PrepareAsync(query);
            var result = await _session.ExecuteAsync(stmt.Bind(conversacionId));

            return result.Select(row => new Mensaje
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
