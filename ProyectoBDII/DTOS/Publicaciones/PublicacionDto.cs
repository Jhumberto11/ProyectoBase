using MongoDB.Bson;

namespace ProyectoBDII.DTOS.Publicaciones
{
    public class PublicacionDto
    {

        public string Id { get; set; }
        public string Title { get; set; }
        public string Descripcion { get; set; }
        public string Condition { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        

    }
}
