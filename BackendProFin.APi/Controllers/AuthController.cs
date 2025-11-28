using Microsoft.AspNetCore.Mvc;
using BackendProFinAPi.Models.DTO; // Asume que LoginDTOcs está aquí
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
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registerDao = await _authService.RegisterUserAsync(registerData);

            if (registerDao == null)
            {
                return BadRequest(new { Message = "El usuario con ese email ya existe." });
            }

            // Opcional: Generar token inmediatamente después del registro
            var token = _authService.GenerateJwtToken(registerDao);

            return Created(string.Empty, new
            {
                UserId = registerDao.Model.Id,
                Email = registerDao.Model.Email,
                Role = registerDao.UserModel.Role,
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

            var loginDao = await _authService.AuthenticateUserAsync(loginDto);

            if (loginDao == null)
            {
                return Unauthorized(new { Message = "Email o contraseña incorrectos." });
            }

            // Generar el Token JWT
            var token = _authService.GenerateJwtToken(loginDao);

            // Devolver el Token y los datos del usuario. El cliente guardará este token.
            return Ok(new
            {
                UserId = loginDao.Model.Id,
                Email = loginDao.Model.Email,
                Role = loginDao.UserModel.Role, // El rol se devuelve para que el frontend lo use
                Token = token
            });
        }
    }
}