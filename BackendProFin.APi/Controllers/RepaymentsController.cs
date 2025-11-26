using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendProFinAPi.Config;
using BackendProFinAPi.Models;

namespace BackendProFinAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RepaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepaymentModel>>> GetRepayments()
        {
            return await _context.Repayments.Include(r => r.Sale).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RepaymentModel>> GetRepayment(int id)
        {
            var repayment = await _context.Repayments.Include(r => r.Sale).FirstOrDefaultAsync(r => r.Id == id);
            if (repayment == null) return NotFound();
            return repayment;
        }

        [HttpPost]
        public async Task<ActionResult<RepaymentModel>> PostRepayment(RepaymentModel repayment)
        {
            _context.Repayments.Add(repayment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRepayment", new { id = repayment.Id }, repayment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepayment(int id, RepaymentModel repayment)
        {
            if (id != repayment.Id) return BadRequest();
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

        [HttpDelete("{id}")]
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