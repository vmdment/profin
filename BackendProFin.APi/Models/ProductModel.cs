using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BackendProFinAPi.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Image { get; set; }

        [Required]
        [MaxLength(150)]
        public string Slug { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        [Required]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }

        public DateTime CreatedAt { get; set; }

        // --- Clave Foránea y Propiedad de Navegación para el Creador (Mantenidas) ---
       
        public int ProductTypeId { get; set; }

        // Propiedad de Navegación: El objeto ProductType real (Relación N:1)
        public ProductTypeModel ProductType { get; set; } = null!;

        // --- Propiedad de Navegación para Ventas (Mantenida) ---
        public ICollection<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
    }
}