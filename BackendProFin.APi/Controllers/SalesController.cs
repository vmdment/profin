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
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // 1. OBTENER LISTA DE VENTAS (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<IEnumerable<SaleModel>>> GetSales()
        {
            // NOTA: Para el rol 'Customer', esta lista debe filtrarse para mostrar 
            // SÓLO las ventas asociadas a su User ID, no todas las ventas del sistema.
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Employee)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .ToListAsync();
        }

        // ---------------------------------------------------------------------
        // 2. OBTENER DETALLE DE VENTA POR ID (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<SaleModel>> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Employee)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .Include(s => s.Warranties)
                .Include(s => s.Repayments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            // Si el usuario es 'Customer', debes asegurar aquí que sale.CustomerId coincide con su ID.

            return sale;
        }

        // ---------------------------------------------------------------------
        // 3. CREAR VENTA (POST) (Rol: Employee)
        // ---------------------------------------------------------------------
        // La creación de ventas es una acción transaccional, debe ser restringida.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<SaleModel>> PostSale(SaleModel sale)
        {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSale", new { id = sale.Id }, sale);
        }

        // ---------------------------------------------------------------------
        // 4. MODIFICAR VENTA (PUT) (Rol: Employee)
        // ---------------------------------------------------------------------
        // La modificación de ventas existentes debe ser solo para personal autorizado.
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> PutSale(int id, SaleModel sale)
        {
            if (id != sale.Id) return BadRequest();
            _context.Entry(sale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // ---------------------------------------------------------------------
        // 5. ELIMINAR VENTA (DELETE) (Rol: Employee)
        // ---------------------------------------------------------------------
        // La eliminación de datos de ventas es una acción sensible.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return NotFound();

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool SaleExists(int id) => _context.Sales.Any(e => e.Id == id);
    }
}