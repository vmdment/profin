using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class SaleModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime SaleDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; } // Ej: Contado, Crédito

        // --- Claves Foráneas y Propiedades de Navegación ---

        // 1. Relación N:1 con CustomerModel (Cliente)
        public int CustomerId { get; set; }
        public CustomerModel Customer { get; set; } = null!;

        // 2. Relación N:1 con EmployeeModel (Empleado/Vendedor)
        public int EmployeeId { get; set; }
        public EmployeeModel Employee { get; set; } = null!;

        // 3. Relación 1:N con RepaymentModel (Abonos)
        public ICollection<RepaymentModel> Repayments { get; set; } = new List<RepaymentModel>();

        // 4. Relación 1:1 o 1:N con WarrantyModel (Garantía)
        public WarrantyModel? Warranty { get; set; }

        // 5. Relación N:M con ProductModel (Usando una tabla intermedia)
        // public ICollection<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
        // En BackendProFinAPi.Models/SaleModel.cs (AJUSTAR ESTA SECCIÓN)

        // ... resto de propiedades

        // 4. Relación N:M con ProductModel (Usando la tabla intermedia SaleDetailModel)
        public ICollection<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
    }
}