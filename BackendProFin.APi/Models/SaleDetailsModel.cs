using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    // Tabla intermedia para la relación N:M entre Venta y Producto
    public class SaleDetailModel
    {
        [Key]
        public int Id { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        // Precio al que se vendió el producto en ese momento
        public decimal UnitPriceAtSale { get; set; }

        // --- Claves Foráneas ---

        // Clave Foránea a la Venta (SaleModel)
        public int SaleId { get; set; }

        // Clave Foránea al Producto (ProductModel)
        public int ProductId { get; set; }

        // --- Propiedades de Navegación ---

        public SaleModel Sale { get; set; } = null!;
        public ProductModel Product { get; set; } = null!;
    }
}