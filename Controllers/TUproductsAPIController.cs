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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TUproductsAPIController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TUproductsAPIController(diveShopperContext context)
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
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTUproduct(int id, TUproduct tUproduct)
        //{
        //    if (id != tUproduct.UproductId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tUproduct).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TUproductExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTUproduct(int id, TUproductsDetailDTO tUproductDetailDTO)
        {
            if (id != tUproductDetailDTO.ProductId)
            {
                return BadRequest(new { message = "商品修改失敗!" });
            }

            // 找到對應的商品
            TUproduct? tUproduct = await _context.TUproducts
                .Include(p => p.TUproductImages) // 確保載入圖片
                .FirstOrDefaultAsync(p => p.UproductId == id);
            if (tUproduct == null)
            {
                return NotFound(new { message = "商品不存在!" });
            }

            tUproduct.ProductName = tUproductDetailDTO.ProductName;
            tUproduct.ProductPrice = tUproductDetailDTO.ProductPrice;
            tUproduct.CategoryId = tUproductDetailDTO.CategoryId;
            tUproduct.ProductDescription = tUproductDetailDTO.ProductDescription;
            tUproduct.ProductStatus = tUproductDetailDTO.ProductStatus;
            tUproduct.ProductConditionId = tUproductDetailDTO.ProductConditionId;
            tUproduct.UpdatedAt = DateTime.Now;
            _context.Entry(tUproduct).State = EntityState.Modified;

            // 更新圖片邏輯
            if (tUproductDetailDTO.TUproductImages != null && tUproductDetailDTO.TUproductImages.Length > 0)
            {
                var existingImage = tUproduct.TUproductImages.OrderBy(img => img.Uimage).ToList();
                for (int i = 0; i < tUproductDetailDTO.TUproductImages.Length; i++)
                {
                    string base64Image = tUproductDetailDTO.TUproductImages[i];

                    if (!string.IsNullOrEmpty(base64Image))
                    {
                        byte[] imageBytes = Convert.FromBase64String(base64Image);

                        // 取得現有圖片                        
                        if (i < existingImage.Count)
                        {
                            // 更新現有圖片
                            existingImage[i].Uimage = imageBytes;
                            _context.Entry(existingImage[i]).State = EntityState.Modified;
                        }
                        else
                        {
                            // 新增新圖片
                            var newImage = new TUproductImage
                            {
                                UproductId = tUproduct.UproductId,
                                Uimage = imageBytes
                            };
                            _context.TUproductImages.Add(newImage);
                        }
                    }
                }
                if (existingImage.Count > tUproductDetailDTO.TUproductImages.Length)
                {
                    var imagesToRemove = existingImage.Skip(tUproductDetailDTO.TUproductImages.Length).ToList();
                    _context.TUproductImages.RemoveRange(imagesToRemove);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TUproductExists(id))
                {
                    return NotFound(new { message = "商品不存在!" });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { message = "商品修改成功!" });
        }


        // POST: api/TUproductsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<TUproduct>> PostTUproduct(TUproduct tUproduct)
        //{
        //    _context.TUproducts.Add(tUproduct);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTUproduct", new { id = tUproduct.UproductId }, tUproduct);
        //}
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> PostTUproduct(TUproductsDetailDTO uproductDetailDTO)
        {
            // 取得目前登入的 UserId
            //var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if (string.IsNullOrEmpty(userIdClaim))
            //{
            //    Console.WriteLine("❌ 無法獲取 UserId，請檢查 JWT Token 是否正確傳遞");
            //    return Unauthorized(new { message = "無效的 Token 或用戶未驗證" });
            //}

            //Console.WriteLine($"✅ 成功獲取 UserId: {userIdClaim}");

            //var userId = int.Parse(userIdClaim);
            // 建立商品
            TUproduct uproduct = new TUproduct
            {
                SellerId = uproductDetailDTO.SellerId,
                ProductName = uproductDetailDTO.ProductName,
                CategoryId = uproductDetailDTO.CategoryId,
                ProductDescription = uproductDetailDTO.ProductDescription,
                ProductPrice = uproductDetailDTO.ProductPrice,
                ProductConditionId = uproductDetailDTO.ProductConditionId,
                ProductStatus = uproductDetailDTO.ProductStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };
            _context.TUproducts.Add(uproduct);
            await _context.SaveChangesAsync(); // 先儲存以取得 id

            // 新增圖片
            if (uproductDetailDTO.TUproductImages != null && uproductDetailDTO.TUproductImages.Length > 0)
            {
                foreach (var base64Image in uproductDetailDTO.TUproductImages)
                {
                    if (!string.IsNullOrEmpty(base64Image))
                    {
                        var uproductImage = new TUproductImage
                        {
                            UproductId = uproduct.UproductId,
                            Uimage = Convert.FromBase64String(base64Image), // 轉換 Base64 為 byte[]
                        };
                        _context.TUproductImages.Add(uproductImage);
                    }
                }
            }
            await _context.SaveChangesAsync(); // 儲存圖片

            return Ok(new { message = "商品新增成功!" });
        }


        // DELETE: api/TUproductsAPI/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTUproduct(int id)
        //{
        //    var tUproduct = await _context.TUproducts.FindAsync(id);
        //    if (tUproduct == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TUproducts.Remove(tUproduct);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool TUproductExists(int id)
        //{
        //    return _context.TUproducts.Any(e => e.UproductId == id);
        //}
        // DELETE: api/TUproductsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTUproduct(int id)
        {
            var tUproduct = await _context.TUproducts
                .Include(p => p.TUproductImages) // 假設有關聯圖片表
                .FirstOrDefaultAsync(p => p.UproductId == id);

            if (tUproduct == null)
            {
                return NotFound(new { message = "刪除商品失敗!" });
            }

            try
            {
                if (tUproduct.TUproductImages != null && tUproduct.TUproductImages.Any())
                {
                    _context.TUproductImages.RemoveRange(tUproduct.TUproductImages);
                }

                _context.TUproducts.Remove(tUproduct);
                await _context.SaveChangesAsync();
                return Ok(new { message = "刪除商品成功!" });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = "刪除商品失敗，可能因為與其他資料有關聯!", error = ex.Message });
            }
        }

        private bool TUproductExists(int id)
        {
            return _context.TUproducts.Any(e => e.UproductId == id);
        }

    }
}
