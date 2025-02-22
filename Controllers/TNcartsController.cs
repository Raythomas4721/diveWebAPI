using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNcartsController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TNcartsController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TNcarts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TNcart>>> GetTNcarts()
        {
            return await _context.TNcarts.ToListAsync();
        }

        // GET: api/TNcarts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TNcart>> GetTNcart(int id)
        {
            var tNcart = await _context.TNcarts.FindAsync(id);

            if (tNcart == null)
            {
                return NotFound();
            }

            return tNcart;
        }

        // PUT: api/TNcarts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTNcart(int id, TNcart tNcart)
        {
            if (id != tNcart.CartId)
            {
                return BadRequest();
            }

            _context.Entry(tNcart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TNcartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TNcarts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TNcart>> PostTNcart(TNcart tNcart)
        {
            _context.TNcarts.Add(tNcart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTNcart", new { id = tNcart.CartId }, tNcart);
        }

        // DELETE: api/TNcarts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTNcart(int id)
        {
            var tNcart = await _context.TNcarts.FindAsync(id);
            if (tNcart == null)
            {
                return NotFound();
            }

            _context.TNcarts.Remove(tNcart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TNcartExists(int id)
        {
            return _context.TNcarts.Any(e => e.CartId == id);
        }
    }
}
