using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBDII.Domain.Models
{
    public class CategoriaAtributo
    {
        [BsonElement("key")]
        public string Key { get; set; } = null!;// atributo a incluir
        [BsonElement("label")]
        public string Label { get; set; } = null!;//
        [BsonElement("type")]
        public string Type { get; set; } = null!; // string, number, boolean, select ->Tipo de dato
        [BsonElement("required")]
        public bool Required { get; set; }
        [BsonElement("options")]
        public List<string>? Options { get; set; }
    }
}
