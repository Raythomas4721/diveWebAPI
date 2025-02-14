using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.DTO;
using System.Drawing.Imaging;
using System.Drawing;

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
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TUproduct>>> GetTUproducts()
        //{
        //    return await _context.TUproducts.ToListAsync();
        //}
        // GET: api/TUproductsAPI
        [HttpGet]
        public async Task<IEnumerable<TUproductsAllDTO>> GetTUproducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6,
            [FromQuery] string? keyword = null,
            [FromQuery] int? categoryId = null)
        {
            var products = await _context.TUproducts
                .Include(p => p.TUproductImages) // 載入商品圖片
                .Where(p => (bool)p.ProductStatus) // 只篩選 ProductStatus 為 true 的商品
                .Where(p => string.IsNullOrEmpty(keyword) || p.ProductName.Contains(keyword) || p.ProductDescription.Contains(keyword)) // 關鍵字搜尋
                .Where(p => !categoryId.HasValue || p.CategoryId == categoryId) // 篩選類別
                .OrderByDescending(p => (p.UpdatedAt ?? p.CreatedAt)) // 依據最新的時間排序
                .Skip((page - 1) * pageSize) // 分頁，跳過前面 (page - 1) * pageSize 筆
                .Take(pageSize) // 取 pageSize 筆數據
                .ToListAsync(); // 執行查詢

            // 在記憶體中處理圖片
            var productList = products.Select(p => new TUproductsAllDTO
            {
                ProductId = p.UproductId,
                SellerId = p.SellerId,
                CategoryId = p.CategoryId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                ProductPrice = p.ProductPrice,
                UpdatedAt = p.UpdatedAt,
                CreatedAt = p.CreatedAt,
                ProductConditionId = p.ProductConditionId,
                ProductStatus = p.ProductStatus,
                TUproductImages = p.TUproductImages
                    .OrderBy(img => img.ProductImagesId) // 確保圖片順序
                    .Select(img => img.Uimage) // 取得 byte[] 圖片資料
                    .FirstOrDefault() is byte[] firstImage
                        ? ConvertToThumbnailBase64(firstImage, 200, 200)
                        : null // 轉換圖片為縮圖
            });

            return productList;
        }
        private string ConvertToThumbnailBase64(byte[] imageData, int width, int height)
        {
            using (var ms = new MemoryStream(imageData))
            {
                using (var image = Image.FromStream(ms))
                {
                    using (var thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero))
                    {
                        using (var thumbnailStream = new MemoryStream())
                        {
                            thumbnail.Save(thumbnailStream, ImageFormat.Png);
                            return Convert.ToBase64String(thumbnailStream.ToArray());
                        }
                    }
                }
            }
        }


        // GET: api/TUproductsAPI/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TUproduct>> GetTUproduct(int id)
        //{
        //    var tUproduct = await _context.TUproducts.FindAsync(id);

        //    if (tUproduct == null)
        //    {
        //        return NotFound();
        //    }

        //    return tUproduct;
        //}
        // GET: api/TUproductsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TUproductsDetailDTO>> GetTUproduct(int id)
        {
            var tUproduct = await _context.TUproducts
                .Include(p => p.TUproductImages) // 假設有關聯的圖片表
                .FirstOrDefaultAsync(p => p.UproductId == id);

            if (tUproduct == null)
            {
                return NotFound();
            }

            var productDTO = new TUproductsDetailDTO
            {
                ProductId = tUproduct.UproductId,
                SellerId = tUproduct.SellerId,
                CategoryId = tUproduct.CategoryId,
                ProductName = tUproduct.ProductName,
                ProductDescription = tUproduct.ProductDescription,
                ProductPrice = tUproduct.ProductPrice,
                UpdatedAt = tUproduct.UpdatedAt,
                CreatedAt = tUproduct.CreatedAt,
                ProductConditionId = tUproduct.ProductConditionId,
                ProductStatus = tUproduct.ProductStatus,
                TUproductImages = tUproduct.TUproductImages
                    .OrderBy(img => img.ProductImagesId) // 確保圖片順序一致
                    .Select(img => Convert.ToBase64String(img.Uimage)) // 將 byte[] 轉為 Base64
                    .ToArray() // 轉為陣列
            };

            return Ok(productDTO);
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
