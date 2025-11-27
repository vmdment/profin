using BackendProFinAPi.Config;
using BackendProFinAPi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BackendProFinAPi.Controllers
{
    // La autorización por defecto es requerida, pero permitimos que los métodos
    // sobreescriban los roles si es necesario.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // 1. OBTENER LISTA (Solo para EMPLEADOS)
        // ---------------------------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Employee")] // Requiere rol de Empleado para ver la lista completa
        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // ---------------------------------------------------------------------
        // 2. OBTENER CLIENTE POR ID (Permitido para CLIENTES y EMPLEADOS)
        // ---------------------------------------------------------------------
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Customer")] // Permite ambos roles
        public async Task<ActionResult<CustomerModel>> GetCustomer(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            // Opcional: Implementar control de acceso para que el cliente solo vea su propio perfil
            /*
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Customer") && currentUserId != id)
            {
                return Forbid();
            }
            */

            return customer;
        }

        // ---------------------------------------------------------------------
        // 3. CREAR CLIENTE (Solo para EMPLEADOS)
        // ---------------------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Employee")] // Solo Empleados pueden añadir nuevos clientes
        public async Task<ActionResult<CustomerModel>> PostCustomer(CustomerModel customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // ---------------------------------------------------------------------
        // 4. ACTUALIZAR CLIENTE (Solo para EMPLEADOS)
        // ---------------------------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")] // Solo Empleados pueden modificar clientes
        public async Task<IActionResult> PutCustomer(string id, CustomerModel customer)
        {
            if (id != customer.Id) return BadRequest();
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // ---------------------------------------------------------------------
        // 5. ELIMINAR CLIENTE (Solo para EMPLEADOS)
        // ---------------------------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")] // Solo Empleados pueden eliminar clientes
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CustomerExists(string id) => _context.Customers.Any(e => e.Id == id);
    }
}