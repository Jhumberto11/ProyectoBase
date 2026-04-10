namespace ProyectoBDII.DTOS.Publicaciones
{
    public class ActualizarPublicacionDto
    {

        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        
        public string Condition { get; set; }
        public List<string> Tags { get; set; }

    }
}
