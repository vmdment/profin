using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendProFinAPi.Models
{
    public class EmployeeModel:PersonModel
    {

        [MaxLength(50)]
        public string Position { get; set; }

        public DateTime HireDate { get; set; }

        // --- Propiedades de Navegación ---

        // Ventas realizadas por este empleado (Relación 1:N)
        public ICollection<SaleModel> Sales { get; set; } = new List<SaleModel>();

        // Si usas una relación 1:1 con UserModel para autenticación:
        // public int UserId { get; set; }
        // public UserModel User { get; set; }
    }
}