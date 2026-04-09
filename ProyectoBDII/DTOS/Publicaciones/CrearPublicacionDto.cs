namespace ProyectoBDII.DTOS.Publicaciones
{
    public class CrearPublicacionDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public string Condition { get; set; } = "used";
        public string CategoryId { get; set; } = null!;
        public string? LocationCity { get; set; } = "San Salvador";
        public string LocationDepartment { get; set; } = "San Salvador";
        public List<string> Tags { get; set; } = new();
        public Dictionary<string, object> Attributes { get; set; } = new();
    }

  
}
