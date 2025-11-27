using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class RepaymentModel
    {
        [Key]
        public int Id { get; set; }

        // Clave Foránea de la Venta
        public int SaleId { get; set; }
        [ForeignKey("SaleId")]
        public SaleModel Sale { get; set; }

        // Monto pagado en esta amortización
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        // Fecha en que se realizó el pago
        public DateTime RepaymentDate { get; set; }

        // 🔑 CAMBIO SUGERIDO: Método de pago
        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; }
    }
}