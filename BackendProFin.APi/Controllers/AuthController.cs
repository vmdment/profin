using Microsoft.AspNetCore.Mvc;
using BackendProFinAPi.Moldels.DTO; // Asume que LoginDTOcs está aquí
using BackendProFinAPi.Services;
using System.Threading.Tasks;

namespace BackendProFinAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyección de IAuthService
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Permite a un nuevo usuario registrarse en el sistema. Por defecto, asigna el rol "Customer".
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] LoginDTOcs registerData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.RegisterUserAsync(registerData);

            if (user == null)
            {
                return BadRequest(new { Message = "El usuario con ese email ya existe." });
            }

            // Opcional: Generar token inmediatamente después del registro
            var token = _authService.GenerateJwtToken(user);

            return Created(string.Empty, new
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = token
            });
        }

        /// <summary>
        /// Autentica a un usuario y le proporciona un token JWT válido.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTOcs loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.AuthenticateUserAsync(loginDto);

            if (user == null)
            {
                return Unauthorized(new { Message = "Email o contraseña incorrectos." });
            }

            // Generar el Token JWT
            var token = _authService.GenerateJwtToken(user);

            // Devolver el Token y los datos del usuario. El cliente guardará este token.
            return Ok(new
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role, // El rol se devuelve para que el frontend lo use
                Token = token
            });
        }
    }
}