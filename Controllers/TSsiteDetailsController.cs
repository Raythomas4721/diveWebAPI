using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.Models.SiteDTO;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TSsiteDetailsController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TSsiteDetailsController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TSsiteDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TSsiteDetail>>> GetTSsiteDetails()
        {
            return await _context.TSsiteDetails.ToListAsync();
        }

        // GET: api/TSsiteDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TSsiteDetail>> GetTSsiteDetail(int id)
        {
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);

            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            return tSsiteDetail;
        }

        // PUT: api/TSsiteDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSsiteDetail(int id, TSsiteDetail tSsiteDetail)
        {
            if (id != tSsiteDetail.SiteId)
            {
                return BadRequest();
            }

            _context.Entry(tSsiteDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSsiteDetailExists(id))
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

        // POST: api/TSsiteDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostTSsiteDetail")]
        public async Task<ActionResult<TSsiteDetail>> PostTSsiteDetail(TSsiteDetail tSsiteDetail)
        {
            _context.TSsiteDetails.Add(tSsiteDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSsiteDetail", new { id = tSsiteDetail.SiteId }, tSsiteDetail);
        }

        [HttpPost("PostTSsiteDetailPhoto")]
        public async Task<IActionResult> PostTSsiteDetailPhoto4564([FromForm] CreateTSsiteDetailDTO dto)
        {
            var tSsiteDetail = new TSsiteDetail
            {
                VenueName = dto.VenueName,
                NumberOfPeople = dto.NumberOfPeople,
                VenueAddress = dto.VenueAddress,
                Detail = dto.Detail,
                Evaluate = dto.Evaluate,
                Collect = dto.Collect
            };

            // 處理上傳的圖片
            if (dto.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.Photo.CopyToAsync(memoryStream);
                    tSsiteDetail.Photo = memoryStream.ToArray(); // 將圖片轉換為 byte[]
                }
            }

            // 將 tSsiteDetail 物件添加到資料庫
            _context.TSsiteDetails.Add(tSsiteDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSsiteDetail", new { id = tSsiteDetail.SiteId }, tSsiteDetail);
        }

        // DELETE: api/TSsiteDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSsiteDetail(int id)
        {
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);
            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            _context.TSsiteDetails.Remove(tSsiteDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TSsiteDetailExists(int id)
        {
            return _context.TSsiteDetails.Any(e => e.SiteId == id);
        }
    }
}
