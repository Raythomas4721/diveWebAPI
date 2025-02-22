using diveWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAllOrdersController : ControllerBase
    {
        private readonly DiveShopperContext _context;
        public UserAllOrdersController(DiveShopperContext context)
        {
            _context = context;
        }
        [HttpGet("allOrders")]
        public IActionResult GetAllOrders([FromQuery] int memberId)
        {
            var nOrders = _context.TNorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new
                {
                    OrderType = "NewProduct",
                    o.OrderId,
                    o.OrderStatus,
                    o.ShipAddress,
                    o.ShipPhone,
                    o.PaymentMethod
                });

            var uOrders = _context.TUorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new
                {
                    OrderType = "UsedProduct",
                    o.OrderId,
                    o.OrderStatusId,
                    o.OrderLogId
                });

            var cOrders = _context.TCorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new
                {
                    OrderType = "Course",
                    o.OrderId,
                    o.OrderDate
                });

            var sOrders = _context.TSorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new
                {
                    OrderType = "SiteRental",
                    o.OrderId,
                    o.SiteId
                });

            //var allOrders = nOrders
            //    .Union(uOrders)
            //    .Union(cOrders)
            //    .Union(sOrders)
            //    .OrderByDescending(o => o.orderId)
            //    .ToList();

            //return Ok(allOrders);
            return Ok(nOrders);
        }

    }
}
