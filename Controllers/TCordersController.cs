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
    public class TCordersController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TCordersController(DiveShopperContext context)
        {
            _context = context;
        }


        // GET: api/TCorders/member/1026
        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<IEnumerable<TCorderDTO>>> GetOrdersByMemberId(int memberId)
        {
            var orders = await _context.TCorders
                .Where(o => o.MemberId == memberId)
                .Include(o => o.Member) // 確保獲取會員資訊
                .Include(o => o.Course)
                .ThenInclude(c => c.CourseCategory) // 獲取課程類別
                .Include(o => o.Course.Level) // 獲取課程等級
                .Include(o => o.Course.Coach) // 獲取教練資訊
                .Select(o => new TCorderDTO
                {
                    OrderId = o.OrderId,
                    MemberId = o.MemberId,
                    MemberName = o.Member.MemberName, // 確保會員名稱被載入
                    CourseId = o.CourseId,
                    CourseName = $"{o.Course.StartAt:MM/dd} {o.Course.CourseCategory.CategoryName} 體驗課程｜免費拍照｜免證照",
                    CategoryName = o.Course.CourseCategory.CategoryName ?? "未分類",
                    LevelName = o.Course.Level.LevelName ?? "無等級",
                    CoachName = o.Course.Coach.CoachName ?? "無教練",
                    CoursePrice = o.CoursePrice,
                    Quantity = o.Quantity,
                    OrderDate = o.OrderDate,
                    OrderStatus = o.OrderStatus,
                    StartAt = o.Course.StartAt
                })
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound(new { message = "沒有找到該會員的訂單紀錄" });
            }

            return Ok(orders);
        }

        // GET: api/TCorders
        [HttpGet]
        public async Task<IEnumerable<TCorderDTO>> GetTCorders()
        {
            return _context.TCorders
                .Include(e => e.Member)
                .Include(e => e.Course)
                .Select(e => new TCorderDTO
                {
                    OrderId = e.OrderId,
                    MemberId = e.MemberId,
                    MemberName = e.Member.MemberName,
                    CourseId = e.CourseId,
                    CourseName = $"{e.Course.StartAt:MM/dd}{e.Course.CourseCategory.CategoryName}體驗課程｜免費拍照｜免證照",
                    CategoryName=e.Course.CourseCategory.CategoryName,
                    CoursePrice = e.CoursePrice,
                    Quantity = e.Quantity,
                    OrderDate = e.OrderDate,
                    OrderStatus = e.OrderStatus
                });
        }

        // PUT: api/TCorders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutTCorder(int id, TCorderDTO orderDTO)
        {
            if (id != orderDTO.OrderId)
            {
                return "修改訂單失敗";
            }
            int memberId = _context.TMmemberLists.FirstOrDefault(e => e.MemberName == orderDTO.MemberName).MemberId;
            TCorder tCorder = await _context.TCorders.FindAsync(id);
            tCorder.MemberId = memberId;
            tCorder.CourseId = orderDTO.CourseId;
            tCorder.CoursePrice = orderDTO.CoursePrice;
            tCorder.Quantity = orderDTO.Quantity;
            tCorder.OrderDate = orderDTO.OrderDate;
            tCorder.OrderStatus = orderDTO.OrderStatus;

            try
            {
                _context.TCorders.Update(tCorder);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TCorderExists(id))
                {
                    return "修改訂單資料庫失敗";
                }
                else
                {
                    throw;
                }
            }

            return $"修改訂單{id}成功";
        }

        private bool TCorderExists(int id)
        {
            return _context.TCorders.Any(e => e.OrderId == id);
        }



        // PUT: api/TCorders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<string> PutTCorder(int id, TCorderDTO orderDTO)
        //{
        //    if (id != orderDTO.OrderId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tCorder).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TCorderExists(id))
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

        // POST: api/TCorders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostTCorder(TCorderDTO orderDTO)
        {
            try
            {
                if (!orderDTO.MemberId.HasValue || !orderDTO.CourseId.HasValue)
                {
                    return BadRequest(new { message = "MemberId 或 CourseId 不能為空" });
                }

                // 建立訂單物件
                TCorder tcorder = new TCorder
                {
                    OrderId = 0,
                    MemberId = orderDTO.MemberId.Value,
                    CourseId = orderDTO.CourseId.Value,
                    CoursePrice = orderDTO.CoursePrice ?? 0,
                    Quantity = orderDTO.Quantity ?? 1,
                    OrderDate = orderDTO.OrderDate ?? DateTime.Now,
                    OrderStatus = orderDTO.OrderStatus ?? false
                };

                _context.TCorders.Add(tcorder);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"訂單已建立，編號: {tcorder.OrderId}", orderId = tcorder.OrderId });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"❌ 資料庫錯誤: {dbEx.InnerException?.Message ?? dbEx.Message}");
                return StatusCode(500, new { message = $"資料庫錯誤: {dbEx.InnerException?.Message ?? dbEx.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 訂單建立失敗: {ex.Message}");
                return StatusCode(500, new { message = $"伺服器錯誤: {ex.Message}" });
            }
        }


        // DELETE: api/TCorders/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTCorder(int id)
        //{
        //    var tCorder = await _context.TCorders.FindAsync(id);
        //    if (tCorder == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TCorders.Remove(tCorder);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool TCorderExists(int id)
        //{
        //    return _context.TCorders.Any(e => e.OrderId == id);
        //}
    }
}
