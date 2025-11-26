using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;

namespace BackendProFinAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WarrantiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarrantyModel>>> GetWarranties()
        {
            return await _context.Warranties.Include(w => w.Sale).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarrantyModel>> GetWarranty(int id)
        {
            var warranty = await _context.Warranties.Include(w => w.Sale).FirstOrDefaultAsync(w => w.Id == id);
            if (warranty == null) return NotFound();
            return warranty;
        }

        [HttpPost]
        public async Task<ActionResult<WarrantyModel>> PostWarranty(WarrantyModel warranty)
        {
            _context.Warranties.Add(warranty);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetWarranty", new { id = warranty.Id }, warranty);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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