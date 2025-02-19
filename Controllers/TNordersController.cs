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
    public class TNordersController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TNordersController(diveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TNorders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TNorder>>> GetTNorders()
        {
            return await _context.TNorders.ToListAsync();
        }

        // GET: api/TNorders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TNorder>> GetTNorder(int id)
        {
            var tNorder = await _context.TNorders.FindAsync(id);

            if (tNorder == null)
            {
                return NotFound();
            }

            return tNorder;
        }

        // PUT: api/TNorders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTNorder(int id, TNorder tNorder)
        {
            if (id != tNorder.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(tNorder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TNorderExists(id))
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

        [HttpPost]
        public async Task<ActionResult<TNorderDTO>> PostTNorder(TNcreateOrderDTO dto)
        {
            // 1) 建立一個 TNorder 物件 (EF Model)
            var tNorder = new TNorder
            {
                MemberId = dto.MemberId,
                PaymentMethod = dto.PaymentMethod,
                ShipAddress = dto.ShipAddress,
                ShipPhone = dto.ShipPhone,
                OrderStatus = "Pending",  // 或預設 "New" / "Processing"
                CreatedDate = DateTime.Now
            };

            // 2) 計算 totalAmount
            decimal total = 0;

            // 3) 將每筆 OrderItems 加入 order.TNorderDetails
            foreach (var item in dto.OrderItems)
            {
                // 計算該明細小計
                decimal lineSubtotal = item.UnitPriceAtOrder * item.Quantity - item.DiscountAmount;

                // 建立一筆 TNorderDetail (EF Model)
                var od = new TNorderDetail
                {
                    ProductvariantsId = item.ProductvariantsId,
                    UnitPriceAtOrder = item.UnitPriceAtOrder,
                    Quantity = item.Quantity,
                    DiscountAmount = item.DiscountAmount,
                    Subtotal = (int?)lineSubtotal,
                };

                // 放到 order 的關聯集合
                tNorder.TNorderDetails.Add(od);

                total += lineSubtotal;
            }

            // 4) 設定 order total
            tNorder.TotalAmount = total;

            // 5) 存到資料庫
            _context.TNorders.Add(tNorder);
            await _context.SaveChangesAsync();

            // 6) 回傳剛建立的訂單資訊 => 轉成 TNorderDTO
            var orderDto = new TNorderDTO
            {
                OrderId = tNorder.OrderId,
                MemberId = tNorder.MemberId,
                PaymentMethod = tNorder.PaymentMethod,
                ShipAddress = tNorder.ShipAddress,
                ShipPhone = tNorder.ShipPhone,
                OrderStatus = tNorder.OrderStatus,
                CreatedDate = tNorder.CreatedDate,
                TotalAmount = tNorder.TotalAmount,
                OrderDetails = tNorder.TNorderDetails.Select(od => new TNorderDetailDTO
                {
                    OrderDetailId = od.OrderDetailsId,
                    ProductvariantsId = od.ProductvariantsId,
                    UnitPriceAtOrder = od.UnitPriceAtOrder,
                    Quantity = od.Quantity,
                    DiscountAmount = od.DiscountAmount,
                    Subtotal = od.Subtotal
                }).ToList()
            };

            return CreatedAtAction(nameof(GetSingleOrder), new { id = tNorder.OrderId }, orderDto);
        }

        // 若想要查詢單筆訂單
        [HttpGet("single/{id}")]
        public async Task<ActionResult<TNorderDTO>> GetSingleOrder(int id)
        {
            var order = await _context.TNorders
                .Include(o => o.TNorderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            // 轉成 DTO
            var orderDto = new TNorderDTO
            {
                OrderId = order.OrderId,
                MemberId = order.MemberId,
                PaymentMethod = order.PaymentMethod,
                ShipAddress = order.ShipAddress,
                ShipPhone = order.ShipPhone,
                OrderStatus = order.OrderStatus,
                CreatedDate = order.CreatedDate,
                TotalAmount = order.TotalAmount,
                OrderDetails = order.TNorderDetails
                    .Select(od => new TNorderDetailDTO
                    {
                        OrderDetailId = od.OrderDetailsId,
                        ProductvariantsId = od.ProductvariantsId,
                        UnitPriceAtOrder = od.UnitPriceAtOrder,
                        Quantity = od.Quantity,
                        DiscountAmount = od.DiscountAmount,
                        Subtotal = od.Subtotal
                    })
                    .ToList()
            };

            return orderDto;
        }


        // DELETE: api/TNorders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTNorder(int id)
        {
            var tNorder = await _context.TNorders.FindAsync(id);
            if (tNorder == null)
            {
                return NotFound();
            }

            _context.TNorders.Remove(tNorder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TNorderExists(int id)
        {
            return _context.TNorders.Any(e => e.OrderId == id);
        }
    }
}
