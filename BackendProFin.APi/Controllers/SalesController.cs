using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;

namespace BackendProFinAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleModel>>> GetSales()
        {
            // Traemos Cliente, Empleado, y los Detalles de la Venta (incluyendo qué producto es)
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Employee)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .ToListAsync();
        }

        // GET: api/Sales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaleModel>> GetSale(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Employee)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Include(s => s.Warranty)
                .Include(s => s.Repayments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            return sale;
        }

        // POST: api/Sales
        [HttpPost]
        public async Task<ActionResult<SaleModel>> PostSale(SaleModel sale)
        {
            // Al crear la venta, si el JSON incluye 'SaleDetails', EF Core los insertará automáticamente.
            // Asegúrate de enviar 'ProductId' en los detalles, no el objeto 'Product' completo para evitar duplicados.
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSale", new { id = sale.Id }, sale);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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