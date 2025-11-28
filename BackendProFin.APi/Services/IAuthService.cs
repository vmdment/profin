using BackendProFinAPi.Models;
using BackendProFinAPi.Models.DTO;
using BackendProFinAPi.Models.DAO;
using BackendProFinAPi.Repositories;
using Microsoft.AspNetCore.Identity; // Necesario para PasswordHasher y PasswordVerificationResult
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration
using Microsoft.IdentityModel.Tokens; // Necesario para SymmetricSecurityKey
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; // Necesario para JwtSecurityTokenHandler
using System.Security.Claims; // Necesario para ClaimTypes
using System.Text;
using System.Threading.Tasks;

namespace BackendProFinAPi.Services
{
    // =========================================================================
    // 1. INTERFAZ (Contrato)
    // =========================================================================
    public interface IAuthService
    {
        // Métodos de utilidad de contraseña
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);

        // Métodos principales de autenticación
        Task<RegisterDAO> RegisterUserAsync(RegisterDTO registerData);
        Task<LoginDAO> AuthenticateUserAsync(LoginDTOcs loginDto);
        string GenerateJwtToken(UpdateDAO user);
    }

    // -------------------------------------------------------------------------

    // =========================================================================
    // 2. IMPLEMENTACIÓN (Clase)
    // =========================================================================
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;

        // 🔑 CORRECCIÓN: Usamos 'object' para desacoplarnos del modelo UserModel
        private readonly PasswordHasher<object> _passwordHasher;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            // Inicializamos el PasswordHasher con 'object'
            _passwordHasher = new PasswordHasher<object>();
        }

        // --- Lógica de Hashing y Verificación de Contraseña ---

        /// <summary>
        /// Hashea una contraseña usando PasswordHasher de ASP.NET Identity.
        /// </summary>
        public string HashPassword(string password)
        {
            // Pasamos 'null' como el objeto de usuario (tipo object)
            return _passwordHasher.HashPassword(null, password);
        }

        /// <summary>
        /// Verifica una contraseña contra un hash almacenado.
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Pasamos 'null' como el objeto de usuario (tipo object)
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

        // --- Lógica de Registro (Usando UserModel) ---

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        public async Task<RegisterDAO> RegisterUserAsync(RegisterDTO registerData)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registerData.Email);
            if (existingUser == null)
            {
                return null; // El usuario ya existe
            }

            // 1. Hashear la contraseña
            string hashedPassword = HashPassword(registerData.Password);

            // 2. Crear el UserModel
            var newUser = new UserModels
            {
                // Asume que los campos Email, Id, etc., se llenan aquí o en el repositorio
                // Estas líneas se han simplificado/comentado en tu código original, 
                // por lo que las mantengo así, asumiendo que tu repositorio las maneja.
                // Email = registerData.Email,
                PasswordHash = hashedPassword,
                Role = "Customer", // ROL POR DEFECTO
                CreatedAt = DateTime.Now
            };
            var newCustomer = new CustomerModel()
            {
                Role = CustomerRole.Standard,
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                Address = registerData.Address,
                Email = registerData.Email,
                PhoneNumber = registerData.PhoneNumber
            };
            // 3. Guardar el usuario
            await _userRepository.AddUserAsync(newUser);
            await _userRepository.AddCustomerAsync(newCustomer);
            return new RegisterDAO()
            {
                Model = newCustomer, Role = "Customer", UserModel = newUser
            };
        }

        // --- Lógica de Autenticación (Login) ---

        /// <summary>
        /// Autentica un usuario verificando la contraseña.
        /// </summary>
        public async Task<LoginDAO> AuthenticateUserAsync(LoginDTOcs loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = VerifyPassword(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return null;
            }

            PersonModel person;
            if (user.Role == "Customer")
                person = await _userRepository.GetCustomerByEmailAsync(loginDto.Email);
            person = await _userRepository.GetEmployeeByEmailAsync(loginDto.Email);
            return new LoginDAO()
            {
                Model = person, Role = user.Role, UserModel = user
            };
        }

        // --- Lógica de Generación de Token JWT ---

        /// <summary>
        /// Genera un JSON Web Token (JWT) para el usuario autenticado.
        /// </summary>
        public string GenerateJwtToken(UpdateDAO updateDao)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // 1. Obtener la clave secreta
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("La clave JWT no está configurada en appsettings.json.");
            }

            var key = Encoding.ASCII.GetBytes(jwtKey);

            // 2. Definir los Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, updateDao.Model.Id),
                new Claim(ClaimTypes.Email, updateDao.Model.Email),
                new Claim(ClaimTypes.Role, updateDao.UserModel.Role ?? "Customer"),
            };

            // 3. Crear las credenciales del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}