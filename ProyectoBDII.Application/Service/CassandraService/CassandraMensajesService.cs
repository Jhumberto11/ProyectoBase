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


        public async Task<List<string>> ObtenerConversacionesUnicasPorUsuario(string usuarioId, DateOnly? fecha)
        {
            var historial = fecha;

            if (historial == null)

            {
                historial = DateOnly.FromDateTime(DateTime.UtcNow);
            }

            DateTime fechatime = historial.Value.ToDateTime(new TimeOnly(0, 0));

            return await _session.ObtenerConversacionesUnicasPorUsuarioAsync(usuarioId, fechatime);
        }


        public async Task<List<Mensaje>> ObtenerMensajesPorConversacion(string conversacionId, int limit)
        {
            return await _session.ObtenerMensajesPorConversacionAsync(conversacionId, limit);

        }

        public async Task MarcarAsRead(string conversacionId, DateTime fechaEnvio, string mensajeId)
        {
            await _session.MarcarMensajeComoLeído(conversacionId, fechaEnvio, mensajeId);
        }

        public async Task<Mensaje> GetMssg(string conversacionId, DateTime fechaEnvio, string mensajeId)
        {
            return await _session.ObtenerMensajeAsync(conversacionId, fechaEnvio, mensajeId);
        }
    }
}
