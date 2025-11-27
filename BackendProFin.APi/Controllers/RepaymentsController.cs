using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims; // Necesario para acceder al ID del usuario del token JWT

namespace BackendProFinAPi.Controllers
{
    // [Authorize] a nivel de clase: Exige un token JWT válido para acceder a cualquier método.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RepaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RepaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // 1. OBTENER LISTA DE AMORTIZACIONES (Roles: Customer, Employee)
        // Se aplica filtrado para clientes.
        // ---------------------------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<IEnumerable<RepaymentModel>>> GetRepayments()
        {
            // OBTENEMOS el ID del usuario actual (del token JWT)
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Incluir la venta (Sale) para poder filtrar por el CustomerId
            var query = _context.Repayments
                .Include(r => r.Sale);

            // FILTRADO DE AUTORIZACIÓN: Si el usuario es un Cliente, filtramos por su ID.
            if (User.IsInRole("Customer"))
            {
                if (currentUserId == null) return Unauthorized();
                // Filtramos las amortizaciones donde la Venta asociada pertenece al usuario logueado.
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<RepaymentModel, SaleModel>)query.Where(r => r.Sale.CustomerId == currentUserId);
            }

            return await query.ToListAsync();
        }

        // ---------------------------------------------------------------------
        // 2. OBTENER AMORTIZACIÓN ESPECÍFICA (Roles: Customer, Employee)
        // Se aplica validación estricta para clientes.
        // ---------------------------------------------------------------------
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<RepaymentModel>> GetRepayment(int id)
        {
            var repayment = await _context.Repayments
                .Include(r => r.Sale)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (repayment == null) return NotFound();

            // LÓGICA DE VALIDACIÓN: Si es un Cliente, debe ser el dueño de la venta asociada.
            if (User.IsInRole("Customer"))
            {
                var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                // Si el ID del cliente de la venta NO coincide con el ID del usuario logueado, prohibir acceso.
                if (repayment.Sale.CustomerId != currentUserId)
                {
                    return Forbid(); // HTTP 403 - Prohibido.
                }
            }

            return repayment;
        }

        // ---------------------------------------------------------------------
        // 3. REGISTRAR PAGO (POST) (Rol: Employee)
        // Este método recibe el campo PaymentMethod automáticamente.
        // ---------------------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<RepaymentModel>> PostRepayment(RepaymentModel repayment)
        {
            // El model binding de ASP.NET Core maneja el nuevo campo PaymentMethod automáticamente.
            _context.Repayments.Add(repayment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRepayment", new { id = repayment.Id }, repayment);
        }

        // ---------------------------------------------------------------------
        // 4. MODIFICAR PAGO (PUT) (Rol: Employee)
        // Este método también maneja el campo PaymentMethod.
        // ---------------------------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> PutRepayment(int id, RepaymentModel repayment)
        {
            if (id != repayment.Id) return BadRequest();
            // El model binding de ASP.NET Core maneja el campo PaymentMethod automáticamente.
            _context.Entry(repayment).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepaymentExists(id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // ---------------------------------------------------------------------
        // 5. ELIMINAR PAGO (DELETE) (Rol: Employee)
        // ---------------------------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteRepayment(int id)
        {
            var repayment = await _context.Repayments.FindAsync(id);
            if (repayment == null) return NotFound();
            _context.Repayments.Remove(repayment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool RepaymentExists(int id) => _context.Repayments.Any(e => e.Id == id);
    }
}