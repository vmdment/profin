using System.ComponentModel.DataAnnotations;

namespace BackendProFinAPi.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        // --- Propiedades de Navegación ---

        // Compras realizadas por este cliente (Relación 1:N)
        public ICollection<SaleModel> Sales { get; set; } = new List<SaleModel>();
    }
}