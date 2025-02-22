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
    public class TSorderController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TSorderController(diveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TSorder
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TSorder>>> GetTSorders()
        {
            return await _context.TSorders.ToListAsync();
        }

        // GET: api/TSorder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TSorder>> GetTSorder(int id)
        {
            var tSorder = await _context.TSorders.FindAsync(id);

            if (tSorder == null)
            {
                return NotFound();
            }

            return tSorder;
        }

        // PUT: api/TSorder/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSorder(int id, TSorder tSorder)
        {
            if (id != tSorder.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(tSorder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSorderExists(id))
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

        // POST: api/TSorder
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TSorder>> PostTSorder(TSorder tSorder)
        {
            _context.TSorders.Add(tSorder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSorder", new { id = tSorder.OrderId }, tSorder);
        }

        // DELETE: api/TSorder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSorder(int id)
        {
            var tSorder = await _context.TSorders.FindAsync(id);
            if (tSorder == null)
            {
                return NotFound();
            }

            _context.TSorders.Remove(tSorder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TSorderExists(int id)
        {
            return _context.TSorders.Any(e => e.OrderId == id);
        }
    }
}
