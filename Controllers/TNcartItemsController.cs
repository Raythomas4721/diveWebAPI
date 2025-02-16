using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.DTO;
using diveWebAPI.Partial;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNcartItemsController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TNcartItemsController(diveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TNcartItems
        [HttpGet]
        public async Task<IEnumerable<TNcartItemDTO>> GetTNcartItems([FromQuery] int? memberId)
        {
            // 先過濾要取的 cart
            var query = _context.TNcartItems.AsQueryable();

            if (memberId.HasValue)
                query = query.Where(c => c.MemberId == memberId.Value);

            // Include -> 先載入 Productvariants
            // ThenInclude -> 再載入 Product
            var cartItems = await query
                .Include(c => c.Productvariants)
        .ThenInclude(v => v.Product)
        .Include(c => c.Productvariants)
        .ThenInclude(v => v.Color)
        .Include(c => c.Productvariants)
        .ThenInclude(v => v.Size)
         .Include(c => c.Productvariants)
        .ThenInclude(v => v.Thickness)
        .Include(c => c.Productvariants)
        .ThenInclude(v => v.Gender)
    .ToListAsync();

            // 最後 select 成 DTO, 把 Product.ImageUrl 帶出
            var result = cartItems.Select(c => new TNcartItemDTO
            {
                CartitemId = c.CartitemId,
                MemberId = c.MemberId,
                UproductId = c.UproductId,
                ProductName = c.ProductName,
                ProductvariantsId = c.ProductvariantsId,
                Quantity = c.Quantity,
                UnitpriceatCart = c.UnitpriceatCart,
                IsLocked = c.IsLocked,
                Condition = c.Condition,
                CreationDate = c.CreationDate,
                UpdatedDate = c.UpdatedDate,
                // 使用 null 條件運算符（?.）避免 null 引用錯誤
                Color = c.Productvariants?.Color?.Color,  // 檢查 Productvariants 和 Color 是否為 null
                Size = c.Productvariants?.Size?.Size,    // 檢查 Productvariants 和 Size 是否為 null
                Thickness = c.Productvariants?.Thickness?.Thickness,  // 檢查 Productvariants 和 Thickness 是否為 null
                Gender = c.Productvariants?.Gender?.Gender,  // 檢查 Productvariants 和 Gender 是否為 null
                ImageUrl = c.Productvariants?.Product?.ImageUrl  // 檢查 Productvariants 和 Product 是否為 null
            });

            return result;
        }

        // GET: api/TNcartItems/5
        [HttpGet("{id}")]
        public async Task<TNcartItemDTO> GetTNcartItem(int id)
        {
            var tNcartItem = await _context.TNcartItems.FindAsync(id);

            if (tNcartItem == null)
            {
                return null;
            }
            TNcartItemDTO cartItemDTO = new TNcartItemDTO
            {
                CartitemId = tNcartItem.CartitemId,
                MemberId= tNcartItem.MemberId,
                UproductId = tNcartItem.UproductId,
                ProductName= tNcartItem.ProductName,
                ProductvariantsId = tNcartItem.ProductvariantsId,
                Quantity = tNcartItem.Quantity,
                UnitpriceatCart = tNcartItem.UnitpriceatCart,
                IsLocked = tNcartItem.IsLocked,
                Condition = tNcartItem.Condition,
                CreationDate = tNcartItem.CreationDate,
                UpdatedDate = tNcartItem.UpdatedDate,
               ImageUrl = tNcartItem.ImageUrl,
                Color = tNcartItem.Color,
                Size = tNcartItem.Size,
                Thickness = tNcartItem.Thickness,
                Gender = tNcartItem.Gender
            };

            return cartItemDTO;
        }

        // PUT: api/TNcartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutTNcartItem(int id, TNcartItemDTO cartItemDTO)
        {
            if (id != cartItemDTO.CartitemId)
            {
                return "修改購物車失敗";
            }
            TNcartItem cartItem = await _context.TNcartItems.FindAsync(id);
            cartItem.MemberId = cartItemDTO.MemberId;
            cartItem.UproductId = cartItemDTO.UproductId;
            cartItem.ProductName = cartItemDTO.ProductName;
            cartItem.ProductvariantsId = cartItemDTO.ProductvariantsId;
            cartItem.Quantity = cartItemDTO.Quantity;
            cartItem.UnitpriceatCart = cartItemDTO.UnitpriceatCart;
            cartItem.IsLocked = cartItemDTO.IsLocked;
            cartItem.Condition = cartItemDTO.Condition;
            cartItem.CreationDate = cartItemDTO.CreationDate;
            cartItem.UpdatedDate = cartItemDTO.UpdatedDate;
            cartItem.ImageUrl = cartItemDTO.ImageUrl;
            cartItem.Color = cartItemDTO.Color;
            cartItem.Size = cartItemDTO.Size;
            cartItem.Thickness = cartItemDTO.Thickness;
            cartItem.Gender = cartItemDTO.Gender;
            
            _context.Entry(cartItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TNcartItemExists(id))
                {
                    return "修改購物車失敗";
                }
                else
                {
                    throw;
                }
            }

            return "修改購物車成功";
        }

        // POST: api/TNcartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<TNcartItemDTO> PostTNcartItem(TNcartItemDTO cartItemDTO)
        {

            TNcartItem cartItem = new TNcartItem
            {

                CartitemId = 0,
               MemberId=cartItemDTO.MemberId,
                UproductId = cartItemDTO.UproductId,
                ProductName = cartItemDTO.ProductName,
                Quantity = cartItemDTO.Quantity,
                ProductvariantsId = cartItemDTO.ProductvariantsId,
                UpdatedDate = cartItemDTO.UpdatedDate,
                IsLocked = cartItemDTO.IsLocked,
                Condition = cartItemDTO.Condition,
                CreationDate = cartItemDTO.CreationDate,
                UnitpriceatCart = cartItemDTO.UnitpriceatCart,
                ImageUrl=cartItemDTO.ImageUrl,
               Color = cartItemDTO.Color,
               Size = cartItemDTO.Size,
               Thickness = cartItemDTO.Thickness,
                Gender = cartItemDTO.Gender
            };
            _context.TNcartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            cartItemDTO.CartitemId = cartItem.CartitemId;
            return cartItemDTO;
        }


        // DELETE: api/TNcartItems/5
        [HttpDelete("{id}")]
        public async Task<string> DeleteTNcartItem(int id)
        {
            var tNcartItem = await _context.TNcartItems.FindAsync(id);
            if (tNcartItem == null)
            {
                return "刪除購物車失敗";
            }
            try
            {
                _context.TNcartItems.Remove(tNcartItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return "刪除購物車失敗";
            }
            return "刪除購物車成功";
        }

        // DELETE: api/TNcartItems/byVariant/5
        [HttpDelete("byVariant/{variantId}")]
        public async Task<ActionResult<string>> DeleteCartItemByVariant(int variantId, [FromQuery] int? memberId)
        {
            // 範例：若你想根據 productvariantsId & memberId 找到對應的購物車項
            // 假設購物車資料表 TNcartItems 裡 productvariantsId、memberId 對應在 columns
            var query = _context.TNcartItems
                                .Where(ci => ci.ProductvariantsId == variantId);

            // 如果也需要判斷是哪個會員的購物車項，就再加 memberId 過濾
            if (memberId.HasValue)
            {
                query = query.Where(ci => ci.MemberId == memberId.Value);
            }

            var cartItem = await query.FirstOrDefaultAsync();
            if (cartItem == null)
            {
                return NotFound("找不到對應的購物車項目 (by variantId)");
            }

            try
            {
                _context.TNcartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("刪除購物車失敗:" + ex.Message);
            }

            return Ok(new { message = "刪除購物車成功" });
        }


        private bool TNcartItemExists(int id)
        {
            return _context.TNcartItems.Any(e => e.CartitemId == id);
        }
    }
}
