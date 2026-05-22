using MarketplaceApi.Models;
using ProyectoBDII.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface.Cassandra_Interfaces
{
    public interface IMessageRepository
    {

        Task EnviarMensajeAsync(Mensaje mensaje);
        Task<List<Mensaje>> ObtenerMensajesPorConversacionAsync(string conversacionId);
        Task<List<string>> ObtenerConversacionesUnicasPorUsuarioAsync(string usuarioId);
        Task ActualizarConversacionUsuario(string usuarioId, string otroUsuarioId, Mensaje mensaje);

        Task MarcarMensajeComoLeído(string conversacionId, DateTime fechaEnvio, string mensajeId);

        Task<Mensaje> ObtenerMensajeAsync(string conversacionId, DateTime fechaEnvio, string mensajeId);

    }
}
