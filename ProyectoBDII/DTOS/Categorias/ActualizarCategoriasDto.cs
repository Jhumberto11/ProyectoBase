namespace ProyectoBDII.DTOS.Categorias
{
    public class ActualizarCategoriasDto
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<AtributosCategoriaDto> AttributesDefinition { get; set; } = new();

    }
}
