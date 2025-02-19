using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.Models.SiteDTO;
using Microsoft.AspNetCore.Hosting; // 引入 IWebHostEnvironment
using System.IO;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TSsiteDetailsController : ControllerBase
    {
        private readonly diveShopperContext _context;
        private readonly IWebHostEnvironment _environment; // 注入 IWebHostEnvironment

        public TSsiteDetailsController(diveShopperContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        //讀取
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
        //修改
        // PUT: api/TSsiteDetails/5 (更新不含照片)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSsiteDetail(int id, TSsiteDetailUpdateDTO tSsiteDetailDto)
        {
            if (id != tSsiteDetailDto.SiteId)
            {
                return BadRequest();
            }

            // 1. 先確認 SiteId 對應的資料是否存在
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);
            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            // 2. 將 DTO 的值更新到現有的實體
            tSsiteDetail.VenueName = tSsiteDetailDto.VenueName;
            tSsiteDetail.NumberOfPeople = tSsiteDetailDto.NumberOfPeople;
            tSsiteDetail.VenueAddress = tSsiteDetailDto.VenueAddress;
            tSsiteDetail.Detail = tSsiteDetailDto.Detail;
            tSsiteDetail.Evaluate = tSsiteDetailDto.Evaluate;
            tSsiteDetail.Collect = tSsiteDetailDto.Collect;

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

        // PUT: api/TSsiteDetails/5/WithPhoto (更新含照片)
        [HttpPut("{id}/WithPhoto")]
        public async Task<IActionResult> PutTSsiteDetailWithPhoto(int id, [FromForm] TSsiteDetailUpdateWithPhotoDTO tSsiteDetailDto)
        {
            if (id != tSsiteDetailDto.SiteId)
            {
                return BadRequest();
            }

            // 1. 先確認 SiteId 對應的資料是否存在
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);
            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            // 2. 將 DTO 的值更新到現有的實體
            tSsiteDetail.VenueName = tSsiteDetailDto.VenueName;
            tSsiteDetail.NumberOfPeople = tSsiteDetailDto.NumberOfPeople;
            tSsiteDetail.VenueAddress = tSsiteDetailDto.VenueAddress;
            tSsiteDetail.Detail = tSsiteDetailDto.Detail;
            tSsiteDetail.Evaluate = tSsiteDetailDto.Evaluate;
            tSsiteDetail.Collect = tSsiteDetailDto.Collect;

            // 3. 處理照片上傳
            if (tSsiteDetailDto.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await tSsiteDetailDto.Photo.CopyToAsync(memoryStream);
                    tSsiteDetail.Photo = memoryStream.ToArray(); // 將圖片轉換為 byte[]
                }
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

        // (Optional) 儲存照片的範例方法
        private async Task<string> SavePhoto(IFormFile photo)
        {
            // 產生一個獨一無二的檔案名稱
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            // 儲存路徑 (請根據您的專案設定調整)
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return "/uploads/" + fileName; // 回傳相對路徑
        }
        //新增
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
        //刪除
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


