using BackendProFinAPi.Models;
using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Config; // Asume que ApplicationDbContext está aquí
using System.Threading.Tasks;

namespace BackendProFinAPi.Repositories
{
    // Implementación del repositorio usando Entity Framework Core.
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ahora retorna ApplicationUser
        public async Task<UserModels> GetUserByEmailAsync(string email)
        {
            // Busca el usuario por email en la tabla Users de Identity
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Ahora acepta ApplicationUser
        public async Task AddUserAsync(UserModels user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}