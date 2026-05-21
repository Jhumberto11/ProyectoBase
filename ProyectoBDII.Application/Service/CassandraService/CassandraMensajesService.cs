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
    }
}
