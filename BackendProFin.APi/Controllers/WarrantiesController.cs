using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // 🔑 Agregado para usar [Authorize]
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BackendProFinAPi.Controllers
{
    // [Authorize] a nivel de clase: Exige un token JWT válido para acceder a cualquier método.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarrantiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // 1. OBTENER LISTA DE GARANTÍAS (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<IEnumerable<WarrantyModel>>> GetWarranties()
        {
            // NOTA: Similar a Ventas, los clientes solo deberían ver sus propias garantías.
            return await _context.Warranties.Include(w => w.Sale).ToListAsync();
        }

        // ---------------------------------------------------------------------
        // 2. OBTENER GARANTÍA POR ID (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<WarrantyModel>> GetWarranty(int id)
        {
            var warranty = await _context.Warranties.Include(w => w.Sale).FirstOrDefaultAsync(w => w.Id == id);
            if (warranty == null) return NotFound();

            // Lógica de validación si es Customer...

            return warranty;
        }

        // ---------------------------------------------------------------------
        // 3. REGISTRAR GARANTÍA (POST) (Rol: Employee)
        // ---------------------------------------------------------------------
        // La creación/registro de garantías es una función de back-office.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<WarrantyModel>> PostWarranty(WarrantyModel warranty)
        {
            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetWarranty", new { id = warranty.Id }, warranty);
        }

        // ---------------------------------------------------------------------
        // 4. MODIFICAR GARANTÍA (PUT) (Rol: Employee)
        // ---------------------------------------------------------------------
        // Solo personal autorizado debe poder modificar una garantía existente.
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> PutWarranty(int id, WarrantyModel warranty)
        {
            if (id != warranty.Id) return BadRequest();
            _context.Entry(warranty).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarrantyExists(id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // ---------------------------------------------------------------------
        // 5. ELIMINAR GARANTÍA (DELETE) (Rol: Employee)
        // ---------------------------------------------------------------------
        // La eliminación de datos de garantía es una acción sensible.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteWarranty(int id)
        {
            var warranty = await _context.Warranties.FindAsync(id);
            if (warranty == null) return NotFound();
            _context.Warranties.Remove(warranty);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool WarrantyExists(int id) => _context.Warranties.Any(e => e.Id == id);
    }
}