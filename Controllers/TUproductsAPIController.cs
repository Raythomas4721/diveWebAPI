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
    public class TUproductsAPIController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TUproductsAPIController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TUproductsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TUproduct>>> GetTUproducts()
        {
            return await _context.TUproducts.ToListAsync();
        }

        // GET: api/TUproductsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TUproduct>> GetTUproduct(int id)
        {
            var tUproduct = await _context.TUproducts.FindAsync(id);

            if (tUproduct == null)
            {
                return NotFound();
            }

            return tUproduct;
        }

        // PUT: api/TUproductsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTUproduct(int id, TUproduct tUproduct)
        {
            if (id != tUproduct.UproductId)
            {
                return BadRequest();
            }

            _context.Entry(tUproduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TUproductExists(id))
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

        // POST: api/TUproductsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TUproduct>> PostTUproduct(TUproduct tUproduct)
        {
            _context.TUproducts.Add(tUproduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTUproduct", new { id = tUproduct.UproductId }, tUproduct);
        }

        // DELETE: api/TUproductsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTUproduct(int id)
        {
            var tUproduct = await _context.TUproducts.FindAsync(id);
            if (tUproduct == null)
            {
                return NotFound();
            }

            _context.TUproducts.Remove(tUproduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TUproductExists(int id)
        {
            return _context.TUproducts.Any(e => e.UproductId == id);
        }
    }
}
