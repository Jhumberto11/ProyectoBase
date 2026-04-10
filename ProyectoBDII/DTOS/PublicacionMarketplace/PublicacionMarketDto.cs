namespace ProyectoBDII.DTOS.PublicacionMarketplace
{
    public class PublicacionMarketDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Descripcion { get; set; }
        public string Condition { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string SelleId { get; set; }
        public List<string> Tags { get; set; }
        public string CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
