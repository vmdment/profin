using System.ComponentModel.DataAnnotations;
using BackendProFinAPi.Models; // Asegúrate de que el namespace del enum sea accesible

namespace BackendProFinAPi.Models
{
    public class CustomerModel : PersonModel
    {
        // --- Nueva Propiedad para el Rol del Cliente ---

        // El 'enum' CustomerRole se usa para tipificar esta propiedad.
        // Por convención, es bueno inicializarlo con un valor por defecto si es aplicable, 
        // o dejar que el valor predeterminado (el primer elemento, que es 'Standard' en este caso) sea asignado.
        [Required] // Opcional: Asegura que el campo sea obligatorio en la base de datos/modelo
        public CustomerRole Role { get; set; } = CustomerRole.Standard;

        // --- Propiedades de Navegación ---

        // Compras realizadas por este cliente (Relación 1:N)
        public ICollection<SaleModel> Sales { get; set; } = new List<SaleModel>();
    }
}