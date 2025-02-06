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
    public class TCcoursesController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TCcoursesController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TCcourses
        [HttpGet]
        public async Task<IEnumerable<TCcourse>> GetTCcourses()
        {
            return await _context.TCcourses.ToListAsync();
        }

        // GET: api/TCcourses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TCcourse>> GetTCcourse(int id)
        {
            var tCcourse = await _context.TCcourses.FindAsync(id);

            if (tCcourse == null)
            {
                return NotFound();
            }

            return tCcourse;
        }

        // PUT: api/TCcourses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTCcourse(int id, TCcourse tCcourse)
        {
            if (id != tCcourse.CourseId)
            {
                return BadRequest();
            }

            _context.Entry(tCcourse).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TCcourseExists(id))
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

        // POST: api/TCcourses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TCcourse>> PostTCcourse(TCcourse tCcourse)
        {
            _context.TCcourses.Add(tCcourse);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTCcourse", new { id = tCcourse.CourseId }, tCcourse);
        }

        // DELETE: api/TCcourses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTCcourse(int id)
        {
            var tCcourse = await _context.TCcourses.FindAsync(id);
            if (tCcourse == null)
            {
                return NotFound();
            }

            _context.TCcourses.Remove(tCcourse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TCcourseExists(int id)
        {
            return _context.TCcourses.Any(e => e.CourseId == id);
        }
    }
}
