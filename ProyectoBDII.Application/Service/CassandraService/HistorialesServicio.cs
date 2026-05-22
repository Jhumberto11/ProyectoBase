using ProyectoBDII.Domain.Interface.Cassandra_Interfaces;
using ProyectoBDII.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Application.Service.CassandraService
{
    public class HistorialesServicio
    {
        private readonly ILoginHistorialRepository _historialRepository;
        public HistorialesServicio(ILoginHistorialRepository hR)
        {
            _historialRepository = hR;
        }

        public async Task<List<HistorialLogin>> GetHistorialLoginByUser(string userId)
        {

            return await _historialRepository.ObtenerHistorialPorUsuarioAsync(userId);
        }
    }
}
