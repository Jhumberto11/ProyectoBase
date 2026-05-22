using Cassandra;
using ProyectoBDII.Domain.Interface.Cassandra_Interfaces;
using ProyectoBDII.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure.Persistencia
{
    public class HistorialLoginRepository : ILoginHistorialRepository
    {


        private readonly ISession _session;

        public HistorialLoginRepository(CassandraContext context)
        {
            _session = context.Session;
        }

        public async Task GuardarLoginAsync(HistorialLogin login)
        {
            var query = @"
            INSERT INTO historial_login (usuario_id, fecha, estado)
            VALUES (?, ?, ?)";

            var stmt = await _session.PrepareAsync(query);
            await _session.ExecuteAsync(stmt.Bind(
                login.UsuarioId,
                login.Fecha,
                login.Estado
            ));
        }

        public async Task<List<HistorialLogin>> ObtenerHistorialPorUsuarioAsync(string usuarioId, int limit = 20)
        {
            var query = $@"
            SELECT usuario_id, fecha, estado
            FROM historial_login
            WHERE usuario_id = ?
            LIMIT {limit}";

            var stmt = await _session.PrepareAsync(query);
            var result = await _session.ExecuteAsync(stmt.Bind(usuarioId));

            return result.Select(row => new HistorialLogin
            {
                UsuarioId = row.GetValue<string>("usuario_id"),
                Fecha = row.GetValue<DateTime>("fecha"),
                Estado = row.GetValue<bool>("estado")
            }).ToList();
        }

    }
}
