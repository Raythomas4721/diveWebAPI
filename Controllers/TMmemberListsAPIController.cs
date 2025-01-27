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
    public class TMmemberListsAPIController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TMmemberListsAPIController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TMmemberListsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TMmemberList>>> GetTMmemberLists()
        {
            return await _context.TMmemberLists.ToListAsync();
        }

        // GET: api/TMmemberListsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TMmemberList>> GetTMmemberList(int id)
        {
            var tMmemberList = await _context.TMmemberLists.FindAsync(id);

            if (tMmemberList == null)
            {
                return NotFound();
            }

            return tMmemberList;
        }

        // PUT: api/TMmemberListsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTMmemberList(int id, TMmemberList tMmemberList)
        {
            if (id != tMmemberList.MemberId)
            {
                return BadRequest();
            }

            _context.Entry(tMmemberList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TMmemberListExists(id))
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

        // POST: api/TMmemberListsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TMmemberList>> PostTMmemberList(TMmemberList tMmemberList)
        {
            _context.TMmemberLists.Add(tMmemberList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTMmemberList", new { id = tMmemberList.MemberId }, tMmemberList);
        }

        // DELETE: api/TMmemberListsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTMmemberList(int id)
        {
            var tMmemberList = await _context.TMmemberLists.FindAsync(id);
            if (tMmemberList == null)
            {
                return NotFound();
            }

            _context.TMmemberLists.Remove(tMmemberList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TMmemberListExists(int id)
        {
            return _context.TMmemberLists.Any(e => e.MemberId == id);
        }
    }
}
