using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class WarrantyModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Details { get; set; } = string.Empty;

        // --- Clave Foránea y Propiedad de Navegación para la Venta ---

        // Clave Foránea (Única, configurada en DbContext para 1:1)
        public int SaleId { get; set; }

        // Propiedad de Navegación (Relación N:1 con SaleModel)
        public SaleModel Sale { get; set; } = null!;
    }
}