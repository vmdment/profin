using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BackendProFinAPi.Models
{
    public class ProductTypeModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Relación 1:N con ProductModel
        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
    }
}
