using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.DTO;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNproductvariantsController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TNproductvariantsController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TNproductvariants?productId=123
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TNprovariantDTO>>> GetTNproductvariants([FromQuery] int? productId)
        {
            // 建立查詢，先從所有變體開始
            IQueryable<TNproductvariant> query = _context.TNproductvariants;

            // 如果有傳入 productId，則依此過濾
            if (productId.HasValue)
            {
                query = query.Where(p => p.ProductId == productId.Value);
            }

            // 轉換成 DTO 並以清單回傳
            var variants = await query.Select(p => new TNprovariantDTO
            {
                ProductvariantsId = p.ProductvariantsId,
                ProductId = p.ProductId,
                SizeId = p.SizeId,
                ColorId = p.ColorId,
                ThicknessId = p.ThicknessId,
                GenderId = p.GenderId,
                Stock = p.Stock
            }).ToListAsync();

            return variants;
        }


        //// GET: api/TNproductvariants?productId=123
        //[HttpGet]
        //public async Task<IEnumerable<TNprovariantDTO>> GetTNproductvariants()
        //{

        //        return _context.TNproductvariants.Select(p => new TNprovariantDTO
        //        {
        //            ProductvariantsId=p.ProductvariantsId,
        //            ProductId=p.ProductId,
        //            SizeId=p.SizeId,
        //            ColorId=p.ColorId,
        //            ThicknessId=p.ThicknessId,
        //            GenderId=p.GenderId,
        //            Stock=p.Stock
        //        });
        //}

        // GET: api/TNproductvariants/5
        [HttpGet("{id}")]
        public async Task<TNprovariantDTO> GetTNproductvariant(int id)
        {
            var tNproductvariant = await _context.TNproductvariants.FindAsync(id);

            if (tNproductvariant == null)
            {
                return null;
            }
            TNprovariantDTO provariantDTO = new TNprovariantDTO
            {
                ProductvariantsId = tNproductvariant.ProductvariantsId,
                ProductId = tNproductvariant.ProductId,
                SizeId = tNproductvariant.SizeId,
                ColorId = tNproductvariant.ColorId,
                ThicknessId = tNproductvariant.ThicknessId,
                GenderId = tNproductvariant.GenderId,
                Stock = tNproductvariant.Stock
            };
            return provariantDTO;
        }

        // PUT: api/TNproductvariants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutTNproductvariant(int id, TNprovariantDTO provariantDTO)
        {
            if (id != provariantDTO.ProductvariantsId)
            {
                return "修改變體失敗";
            }
            TNproductvariant productvariant = await _context.TNproductvariants.FindAsync(id);
            productvariant.ProductId = provariantDTO.ProductId;
            productvariant.SizeId = provariantDTO.SizeId;
            productvariant.ColorId = provariantDTO.ColorId;
            productvariant.ThicknessId = provariantDTO.ThicknessId;
            productvariant.GenderId = provariantDTO.GenderId;
            productvariant.Stock = provariantDTO.Stock;
            _context.Entry(productvariant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TNproductvariantExists(id))
                {
                    return "修改變體失敗";
                }
                else
                {
                    throw;
                }
            }

            return "修改變體成功";
        }

        // POST: api/TNproductvariants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<TNprovariantDTO> PostTNproductvariant(TNprovariantDTO provariantDTO)
        {
            TNproductvariant productvariant = new TNproductvariant
            {
                ProductvariantsId = 0,
                ProductId = provariantDTO.ProductId,
                SizeId = provariantDTO.SizeId,
                ColorId = provariantDTO.ColorId,
                ThicknessId = provariantDTO.ThicknessId,
                GenderId = provariantDTO.GenderId,
                Stock = provariantDTO.Stock
            };
            _context.TNproductvariants.Add(productvariant);
            await _context.SaveChangesAsync();
            provariantDTO.ProductvariantsId = productvariant.ProductvariantsId;
            return provariantDTO;
        }

        // DELETE: api/TNproductvariants/5
        [HttpDelete("{id}")]
        public async Task<string> DeleteTNproductvariant(int id)
        {
            var tNproductvariant = await _context.TNproductvariants.FindAsync(id);
            if (tNproductvariant == null)
            {
                return "刪除變體失敗";
            }
            try
            {
                _context.TNproductvariants.Remove(tNproductvariant);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return "刪除變體失敗";
            }


            return "刪除變體成功";
        }

        private bool TNproductvariantExists(int id)
        {
            return _context.TNproductvariants.Any(e => e.ProductvariantsId == id);
        }



        // GET: /api/TNproductvariants/sizes?productId=123
        [HttpGet("sizes")]
        public IEnumerable<SizeDTO> GetSizes([FromQuery] int productId)
        {
            var q =
                from s in _context.TNsizes
                join v in _context.TNproductvariants on s.SizeId equals v.SizeId
                where v.ProductId == productId
                group v by new { s.SizeId, s.Size } into g
                select new SizeDTO
                {
                    SizeId = g.Key.SizeId,
                    Size = g.Key.Size,
                    hasStock = g.Sum(x => x.Stock) > 0
                };

            // 注意: 這段只會回傳在 變體表中 真正有對應 productId & colorId 的記錄
            // 如果 TNcolors 裡有其他沒出現在 variants 的 color，這裡不會出現
            // 你可以視需求, 要連空白也回傳 ?

            return q.ToList();
        }


        // GET: /api/TNproductvariants/colors?productId=123
        [HttpGet("colors")]
        public IEnumerable<ColorDTO> GetColors([FromQuery] int productId)
        {
            // 從資料表中，join 到變體、並篩選 productId，取得該商品所有 colorId
            // 並同時計算是否 stock > 0 (hasStock = true/false)

            // 假設 TNcolors 表有 ColorId, Color (名稱) ...
            // TNproductvariants 表 有 ProductId, ColorId, Stock ...
            // 這裡示範使用 GroupBy 方式來判斷「此 colorId 在此商品下的庫存總量是否 > 0」

            var query =
                from c in _context.TNcolors
                join v in _context.TNproductvariants on c.ColorId equals v.ColorId
                where v.ProductId == productId
                group v by new { c.ColorId, c.Color } into g
                select new ColorDTO
                {
                    ColorId = g.Key.ColorId,
                    Color = g.Key.Color,
                    // 看你 DB 欄位，若是 c.Color => 這裡改成 ColorName = g.Key.Color
                    // hasStock => 只要此 colorId 的 stock 加總 > 0，就表示有庫存
                    hasStock = g.Sum(x => x.Stock) > 0
                };

            // 注意: 這段只會回傳在 變體表中 真正有對應 productId & colorId 的記錄
            // 如果 TNcolors 裡有其他沒出現在 variants 的 color，這裡不會出現
            // 你可以視需求, 要連空白也回傳 ?

            return query.ToList();
        }



        [HttpPost("addCart")]
        public async Task<IActionResult> AddCart([FromBody] TNaddcartDTO dto)
        {
            // 1) 找 variant
            var variant = await _context.TNproductvariants
                .Include(v => v.Product)
                .Include(v => v.Color)      // 加這幾行
    .Include(v => v.Size)
    .Include(v => v.Thickness)
    .Include(v => v.Gender)
                .FirstOrDefaultAsync(v =>
                    v.ProductId == dto.ProductId &&
                    v.ColorId == dto.ColorId &&
                    v.SizeId == dto.SizeId &&
                    v.ThicknessId == dto.ThicknessId &&
                    v.GenderId == dto.GenderId
                );
            if (variant == null)
                return Ok(new { success = false, message = "無此變體" });

            // 2) 庫存檢查
            if (variant.Stock < dto.Quantity)
                return Ok(new { success = false, message = $"庫存不足，目前僅剩 {variant.Stock} 件" });

            // 3) 從 variant 或 product 取得名/價
            var productName = variant.Product.ProductName; // e.g. if variant has a navigation "Product"
            var price = variant.Product.UnitPrice; // or variant.UnitPrice, or variant.Product.Price

            // 查詢顏色、尺寸、厚度、款式名稱
            var colorName = variant.Color?.Color;
            var sizeName = variant.Size?.Size;
            var thicknessName = variant.Thickness?.Thickness;
            var genderName = variant.Gender?.Gender;
             var imageUrl = variant.Product?.ImageUrl;


            // 注意：**這裡就不進行購物車新增**，僅回傳成功 & 需要的資訊給前端
            return Ok(new
            {
                success = true,
                message = "庫存充足，可加入購物車",
                productvariantsId = variant.ProductvariantsId,
                price,
                productName,
                colorName,
                sizeName,
                thicknessName,
                genderName,
                imageUrl
            });
        }
    }
}



