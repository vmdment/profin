using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;

[Route("api/[controller]")]
[ApiController]
public class ProductTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductTypeModel>>> GetTypes()
    {
        return await _context.ProductTypes
            .Include(t => t.Products)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ProductTypeModel>> Post(ProductTypeModel type)
    {
        _context.ProductTypes.Add(type);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTypes), new { id = type.Id }, type);
    }
}
