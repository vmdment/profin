using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } // Único, configurado en DbContext

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } // Ej: Admin, Vendedor

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // --- Propiedades de Navegación ---

        // Productos que este usuario ha creado/registrado (Relación 1:N)
        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();

        // Si Employee y User son la misma entidad lógica con diferentes roles/propiedades, 
        // puedes usar una relación 1:1.
        // public EmployeeModel? Employee { get; set; }
    }
}