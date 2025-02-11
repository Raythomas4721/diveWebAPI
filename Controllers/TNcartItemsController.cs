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
            IQueryable<TNcartItem> query = _context.TNcartItems;
            if (memberId.HasValue)
            {
                query = query.Where(c => c.MemberId == memberId.Value);
            }

            // 再 select 成 DTO
            return query.Select(c => new TNcartItemDTO
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
                ImageUrl = c.ImageUrl,
            });
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
                ImageUrl=cartItemDTO.ImageUrl
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

        private bool TNcartItemExists(int id)
        {
            return _context.TNcartItems.Any(e => e.CartitemId == id);
        }
    }
}
