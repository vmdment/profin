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
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // 1. OBTENER LISTA DE PRODUCTOS (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        // El acceso a la lista de productos debe ser consultable por los clientes.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.ProductType)
                .ToListAsync();
        }

        // ---------------------------------------------------------------------
        // 2. OBTENER PRODUCTO POR ID (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        // Un cliente o un empleado puede ver el detalle de un producto específico.
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        // ---------------------------------------------------------------------
        // 3. CREAR PRODUCTO (Rol: Employee)
        // ---------------------------------------------------------------------
        // Solo los empleados pueden crear nuevos productos en el catálogo.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<ProductModel>> PostProduct(ProductModel product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // ---------------------------------------------------------------------
        // 4. ACTUALIZAR PRODUCTO (Rol: Employee)
        // ---------------------------------------------------------------------
        // Solo los empleados pueden modificar los productos existentes.
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
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

        // ---------------------------------------------------------------------
        // 5. ELIMINAR PRODUCTO (Rol: Employee)
        // ---------------------------------------------------------------------
        // Solo los empleados pueden eliminar productos.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
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