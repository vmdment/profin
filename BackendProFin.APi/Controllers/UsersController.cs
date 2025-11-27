using BackendProFinAPi.Moldels.DTO; // Contiene LoginDTOcs
using BackendProFinAPi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")] // api/user
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;

    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Endpoint para registrar un nuevo usuario (utiliza LoginDTOcs como datos de entrada).
    /// Aquí ocurre el Hashing con BCrypt.
    /// </summary>
    [HttpPost("register")] // POST api/user/register
    public async Task<IActionResult> Register([FromBody] LoginDTOcs registerData)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // El servicio maneja la creación del UserModel y el Hashing de la contraseña (BCrypt)
        var newUser = await _authService.RegisterUserAsync(registerData);

        if (newUser == null)
        {
            return Conflict(new { Message = "El correo electrónico ya está registrado." });
        }

        // Generar token después del registro
        var token = _authService.GenerateJwtToken(newUser);

        return Created($"api/user/{newUser.Id}", new
        {
            Message = "Registro exitoso.",
            Token = token,
            UserId = newUser.Id
        });
    }

    // ... Otros métodos para gestionar usuarios ...
}