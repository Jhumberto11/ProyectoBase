using ProyectoBDII.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Interface.Cassandra_Interfaces
{
    public interface ILoginHistorialRepository
    {
        Task GuardarLoginAsync(HistorialLogin login);

        Task<List<HistorialLogin>> ObtenerHistorialPorUsuarioAsync(string usuarioId, int limit); 

    }
}
