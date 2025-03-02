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
using Microsoft.CodeAnalysis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<IEnumerable<TUproductsAllDTO>> GetTUproducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 8,
            [FromQuery] string? keyword = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? sort = null) // 新增排序參數
        {
            //var products = await _context.TUproducts
            //    .Include(p => p.TUproductImages) // 載入商品圖片
            //    .Include(p => p.Seller)
            //    .Where(p => (bool)p.ProductStatus) // 只篩選 ProductStatus 為 true 的商品
            //    .Where(p => string.IsNullOrEmpty(keyword) || p.ProductName.Contains(keyword) || p.ProductDescription.Contains(keyword)) // 關鍵字搜尋
            //    .Where(p => !categoryId.HasValue || p.CategoryId == categoryId) // 篩選類別
            //    .OrderByDescending(p => (p.UpdatedAt ?? p.CreatedAt)) // 依據最新的時間排序
            //    .Skip((page - 1) * pageSize) // 分頁，跳過前面 (page - 1) * pageSize 筆
            //    .Take(pageSize) // 取 pageSize 筆數據
            //    .ToListAsync(); // 執行查詢
                                // **先初始化 query**
            var query = _context.TUproducts
                .Include(p => p.TUproductImages) // 載入商品圖片
                .Include(p => p.Seller)
                .Where(p => (bool)p.ProductStatus) // 只篩選 ProductStatus 為 true 的商品
                .Where(p => string.IsNullOrEmpty(keyword) || p.ProductName.Contains(keyword) || p.ProductDescription.Contains(keyword)) // 關鍵字搜尋
                .Where(p => !categoryId.HasValue || p.CategoryId == categoryId); // 篩選類別

            // **加入排序邏輯**
            switch (sort)
            {
                case "price-asc":
                    query = query.OrderBy(p => p.ProductPrice);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(p => p.ProductPrice);
                    break;
                case "date-asc":
                    query = query.OrderBy(p => p.CreatedAt);
                    break;
                case "date-desc":
                default:
                    query = query.OrderByDescending(p => (p.UpdatedAt ?? p.CreatedAt)); // 預設最新商品在前
                    break;
            }
           

            // **執行分頁並查詢資料**
            var products = await query
                .Skip((page - 1) * pageSize) // 分頁
                .Take(pageSize) // 取指定筆數
                .ToListAsync();

            // 在記憶體中處理圖片
            var productList = products.Select(p => new TUproductsAllDTO
            {
                ProductId = p.UproductId,
                SellerName = p.Seller.MemberName,
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
        [HttpGet("myProducts")]
        [Authorize]
        public async Task<IActionResult> GetMyProducts()
        {
            // 確保用戶已登入
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { message = "未授權，請先登入" });
            }

            // 取得 `userId`
            var userIdClaim = User.FindFirst("userId");

            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return Unauthorized(new { message = "無法獲取用戶 ID，請重新登入" });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest(new { message = "用戶 ID 格式錯誤" });
            }

            // 查詢當前會員商品
            var myProducts = await _context.TUproducts
                .Where(p => p.SellerId == userId)
                .Include(p => p.TUproductImages)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var productListDTO = myProducts.Select(p => new TUproductsListDTO
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
                    .OrderBy(img => img.ProductImagesId)
                    .Select(img => img.Uimage)
                    .FirstOrDefault() is byte[] firstImage
                        ? ConvertToThumbnailBase64(firstImage, 200, 200)
                        : null
            });
            return Ok(productListDTO);
        }


        // PUT: api/TUproductsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

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

            // 確保圖片不包含 null
            if (tUproductDetailDTO.TUproductImages != null)
            {
                tUproductDetailDTO.TUproductImages = tUproductDetailDTO.TUproductImages
                    .Where(img => !string.IsNullOrEmpty(img))
                    .ToArray();
            }

            // 更新圖片邏輯
            if (tUproductDetailDTO.TUproductImages != null && tUproductDetailDTO.TUproductImages.Length > 0)
            {
                //var existingImage = tUproduct.TUproductImages.OrderBy(img => img.Uimage).ToList();
                var existingImage = tUproduct.TUproductImages.OrderBy(img => img.ProductImagesId).ToList();// 以 ProductImagesId 排序
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
                // 刪除多餘圖片
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

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> PostTUproduct(TUproductsDetailDTO uproductDetailDTO)
        {
            // 建立商品
            TUproduct uproduct = new TUproduct
            {
                //SellerId = userId,

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

        [HttpPost("addCart")]
        public async Task<IActionResult> AddCart([FromBody] TNaddcartDTO dto)
        {
            // 1) 找 variant
            var product = await _context.TUproducts
                .FirstOrDefaultAsync(v =>
                    v.UproductId == dto.ProductId
                );           
            if (product == null)
            {
                return Ok(new { success = false, message = "無此變體" });

            }

            // 2) 庫存檢查


            // 3) 從 variant 或 product 取得名/價
            var productName = product.ProductName; // e.g. if variant has a navigation "Product"
            var price = product.ProductPrice; // or variant.UnitPrice, or variant.Product.Price
            var imageUrl = product.TUproductImages;


            // 注意：**這裡就不進行購物車新增**，僅回傳成功 & 需要的資訊給前端
            return Ok(new
            {
                success = true,
                message = "已加入購物車",
                product.UproductId,
                price,
                productName,
                imageUrl
            });
        }

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
