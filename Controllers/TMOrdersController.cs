using diveWebAPI.Models;
using diveWebAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TMOrdersController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TMOrdersController(diveShopperContext context)
        {
            _context = context;
        }

        [HttpGet("Orders")]
        public IActionResult GetAllOrders([FromQuery] int memberId)
        {
            var nOrders = _context.TNorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "全新商品",
                    OrderId = o.OrderId,
                    OrderStatus = o.OrderStatus,
                    ShipAddress = o.ShipAddress,
                    ShipPhone = o.ShipPhone,
                    PaymentMethod = o.PaymentMethod,
                    OrderStatusId = null,    
                    OrderLogId = null,       
                    OrderDate = null,        
                    SiteId = null           
                });

            var uOrders = _context.TUorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "二手商品",
                    OrderId = o.OrderId,
                    OrderStatus = null,      
                    ShipAddress = null,      
                    ShipPhone = null,        
                    PaymentMethod = null,    
                    OrderStatusId = o.OrderStatusId,
                    OrderLogId = o.OrderLogId,
                    OrderDate = null,        
                    SiteId = null            
                });

            var cOrders = _context.TCorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "潛水課程",
                    OrderId = o.OrderId,
                    OrderStatus = null,      
                    ShipAddress = null,      
                    ShipPhone = null,        
                    PaymentMethod = null,    
                    OrderStatusId = null,    
                    OrderLogId = null,       
                    OrderDate = o.OrderDate,
                    SiteId = null            
                });

            var sOrders = _context.TSorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "場地租借",
                    OrderId = o.OrderId,
                    OrderStatus = null,      
                    ShipAddress = null,      
                    ShipPhone = null,        
                    PaymentMethod = null,    
                    OrderStatusId = null,    
                    OrderLogId = null,       
                    OrderDate = null,        
                    SiteId = o.SiteId
                });

            var allOrders = nOrders
                .Union(uOrders)
                .Union(cOrders)
                .Union(sOrders)
                .OrderByDescending(o => o.OrderId)
                .ToList();

            return Ok(allOrders);
        }
    }
}