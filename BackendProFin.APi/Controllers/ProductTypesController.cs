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
    // [Authorize] a nivel de clase: Exige un token JWT válido para cualquier método.
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // 1. OBTENER LISTA DE TIPOS (Roles: Customer, Employee)
        // ---------------------------------------------------------------------
        // Ambos roles deben poder consultar los tipos de productos disponibles.
        [HttpGet]
        [Authorize(Roles = "Employee,Customer")]
        public async Task<ActionResult<IEnumerable<ProductTypeModel>>> GetTypes()
        {
            return await _context.ProductTypes
                .Include(t => t.Products)
                .ToListAsync();
        }

        // ---------------------------------------------------------------------
        // 2. CREAR TIPO (Rol: Employee)
        // ---------------------------------------------------------------------
        // Solo los empleados pueden modificar o añadir tipos de productos.
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<ProductTypeModel>> Post(ProductTypeModel type)
        {
            _context.ProductTypes.Add(type);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTypes), new { id = type.Id }, type);
        }

        // *********************************************************************
        // NOTA: Se asume que no hay métodos PUT/DELETE, pero si los añades, 
        // también deberían llevar [Authorize(Roles = "Employee")]
        // *********************************************************************
    }
}