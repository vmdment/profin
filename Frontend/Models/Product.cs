
namespace FrontendProFin.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int Cant { get; set; }
    }
}