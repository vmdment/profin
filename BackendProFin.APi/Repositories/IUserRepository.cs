using BackendProFinAPi.Models;
using System.Threading.Tasks;

namespace BackendProFinAPi.Repositories
{
    // Define el contrato para el acceso a datos de los usuarios.
    // 🔑 CAMBIO: Ahora usa UserModel, la clase que extiende IdentityUser.
    public interface IUserRepository
    {
        // Usado para verificar si existe un usuario y para el login.
        Task<UserModels> GetUserByEmailAsync(string email);

        Task<CustomerModel> GetCustomerByEmailAsync(string email);

        Task<EmployeeModel> GetEmployeeByEmailAsync(string email);

        // Usado para crear un nuevo usuario.
        Task AddUserAsync(UserModels user);
        Task AddCustomerAsync(CustomerModel customer);
    }
}