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
        private readonly diveShopperContext _context;

        public TCordersController(diveShopperContext context)
        {
            _context = context;
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
        public async Task<String> PostTCorder(TCorderDTO orderDTO)
        {
            //int memberId = _context.TCcourseLevels.FirstOrDefault(e => e.== courseDTO.LevelName).LevelId;

            TCorder tcorder = new TCorder { 
                OrderId = 0,
                MemberId = orderDTO.MemberId,
                CourseId= orderDTO.CourseId,
                CoursePrice= orderDTO.CoursePrice,
                Quantity= orderDTO.Quantity,
                OrderDate= orderDTO.OrderDate,
                OrderStatus=orderDTO.OrderStatus

            };               
            _context.TCorders.Add(tcorder);
            await _context.SaveChangesAsync();
            return $"訂單編號:{tcorder.OrderId}";
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
