namespace ProyectoBDII.DTOS.Categorias
{
    public class AtributosCategoriaDto
    {
        public string Key { get; set; } = null!;
        public string Label { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool Required { get; set; }
        public List<string>? Options { get; set; }
    }
}
