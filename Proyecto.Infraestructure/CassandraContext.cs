using Cassandra;
using Microsoft.Extensions.Configuration;
using ProyectoBDII.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Infraestructure
{
    public class CassandraContext
    {
        public ISession Session { get; }

        public CassandraContext(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Cassandra").Get<CassandraSettings>();

            var cluster = Cluster.Builder()
                .AddContactPoint(settings.ContactPoint)
                .WithPort(settings.Port)
                .Build();

            Session = cluster.Connect(settings.Keyspace);
        }
    }
}
