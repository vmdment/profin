using System.ComponentModel.DataAnnotations;

namespace BackendProFinAPi.Models.DTO
{
    public class LoginDTOcs
    {
        [Required]
        [EmailAddress] // Puedes cambiar a Username si lo usas para login
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
