using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class RepaymentModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime RepaymentDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        // --- Clave Foránea y Propiedad de Navegación para la Venta ---

        // Clave Foránea
        public int SaleId { get; set; }

        // Propiedad de Navegación (Relación N:1 con SaleModel)
        public SaleModel Sale { get; set; } = null!;
    }
}