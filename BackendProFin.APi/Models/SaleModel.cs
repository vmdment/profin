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
        public string PaymentMethod { get; set; }

        // Customer
        public string CustomerId { get; set; }
        public CustomerModel Customer { get; set; } = null!;

        // Employee
        public string EmployeeId { get; set; }
        public EmployeeModel Employee { get; set; } = null!;

        // Repayments (1:N)
        public ICollection<RepaymentModel> Repayments { get; set; } = new List<RepaymentModel>();

        // Warranties (1:N) ✔ CORREGIDO
        public ICollection<WarrantyModel> Warranties { get; set; } = new List<WarrantyModel>();

        // SaleDetails (1:N)
        public ICollection<SaleDetailModel> SaleDetails { get; set; } = new List<SaleDetailModel>();
    }
}
