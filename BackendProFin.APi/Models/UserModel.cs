using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class UserModels
    {
        [Key]
        [MaxLength(15)]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } // Único, configurado en DbContext

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string Role { get; set; } // Ej: Admin, Vendedor

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // --- Propiedades de Navegación ---

        
       

    }
}