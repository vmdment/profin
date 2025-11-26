using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;

namespace BackendProFinAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        {
            // Incluimos datos relacionados (Tipo y Creador) para una respuesta más completa
            return await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Creator)
                .ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProduct(ProductModel product)
        {
            // Validamos que existan las referencias foráneas si es necesario
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductModel product)
        {
            if (id != product.Id) return BadRequest();
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id)) return NotFound();
                else throw;
            }
            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductExists(int id) => _context.Products.Any(e => e.Id == id);
    }
}