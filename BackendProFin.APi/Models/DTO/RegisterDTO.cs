using System.ComponentModel.DataAnnotations;

namespace BackendProFinAPi.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress] // Puedes cambiar a Username si lo usas para login
        public string Email { get; set; }

        [Required] public string Password { get; set; }
        

        [Required] [MaxLength(100)] public string FirstName { get; set; }

        [Required] [MaxLength(100)] public string LastName { get; set; }
        
        [Required] public string PhoneNumber { get; set; }

        [Required] public string Address { get; set; }
    }
}
