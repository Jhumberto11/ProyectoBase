namespace ProyectoBDII.DTOS.Categorias
{
    public class CrearCategoriasDto
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public List<AtributosCategoriaDto> AttributesDefinition { get; set; } = new();
    }
}
