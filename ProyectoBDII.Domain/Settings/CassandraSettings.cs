using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Settings
{
    public class CassandraSettings
    {

        public string ContactPoint { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Keyspace { get; set; } = string.Empty;
    }
}
