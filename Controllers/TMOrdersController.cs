using diveWebAPI.Models;
using diveWebAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore; // 加入 Include 支持

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TMOrdersController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TMOrdersController(DiveShopperContext context)
        {
            _context = context;
        }

        private static string GetColorName(string rgb)
        {
            if (string.IsNullOrEmpty(rgb)) return "無選擇";
            Debug.WriteLine($"處理的 RGB 值: {rgb}");
            return rgb switch
            {
                "rgb(56, 124, 220)" => "藍色",
                "rgb(0, 0, 255)" => "藍色",
                "rgb(0, 0, 0)" => "黑色",
                "rgb(255, 255, 255)" => "白色",
                "rgb(246, 0, 123)" => "粉紅色",
                "rgb(234, 234, 234)" => "淺灰色",
                "無選擇" => "無選擇",
                _ => "未知"
            };
        }

        [HttpGet("Orders")]
        public IActionResult GetAllOrders([FromQuery] int memberId)
        {
            // 全新商品訂單 + 細節
            var nOrders = _context.TNorders
                .Where(o => o.MemberId == memberId)
                .Include(o => o.TNorderDetails) // 預先載入 TNorderDetails
                .ToList() // 先將主訂單和細節載入記憶體
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
                    OrderDate = o.CreatedDate,
                    SiteId = null,
                    Details = o.TNorderDetails
                        .Select(d =>
                        {
                            var variant = _context.TNproductvariants
                                .FirstOrDefault(pv => pv.ProductvariantsId == d.ProductvariantsId);
                            var productName = variant != null
                                ? _context.TNproducts
                                    .Where(p => p.ProductId == variant.ProductId)
                                    .Select(p => p.ProductName)
                                    .FirstOrDefault() ?? "未知商品"
                                : "未知商品";
                            var colorRgb = variant != null && variant.ColorId.HasValue
                                ? _context.TNcolors
                                    .Where(c => c.ColorId == variant.ColorId)
                                    .Select(c => c.Color)
                                    .FirstOrDefault() ?? "無選擇"
                                : "無選擇";
                            var colorName = GetColorName(colorRgb);
                            Debug.WriteLine($"OrderId: {o.OrderId}, ProductvariantsId: {d.ProductvariantsId}, ProductName: {productName}, ColorRgb: {colorRgb}, ColorName: {colorName}");
                            return new OrderDetailDTO
                            {
                                ItemName = $"{productName} - {colorName}",
                                Quantity = d.Quantity.HasValue ? (int)d.Quantity.Value : 0,
                                TotalPrice = d.Subtotal.HasValue ? (int)d.Subtotal.Value : 0
                            };
                        }).ToList()
                }).ToList();

            // 二手商品訂單 + 細節
            var uOrders = _context.TUorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "二手商品",
                    OrderId = o.OrderId,
                    OrderStatus = o.OrderStatus,
                    ShipAddress = null,
                    ShipPhone = null,
                    PaymentMethod = o.PaymentMethod,
                    OrderStatusId = o.OrderStatusId,
                    OrderLogId = o.OrderLogId,
                    OrderDate = o.OrderDate,
                    SiteId = null,
                    Details = o.TUorderDetails
                        .Select(d => new OrderDetailDTO
                        {
                            ItemName = _context.TUproducts
                                .Where(up => up.UproductId == d.ProductId)
                                .Select(up => up.ProductName)
                                .FirstOrDefault() ?? "未知二手商品",
                            Quantity = d.Quantity.HasValue ? (int)d.Quantity.Value : 0,
                            TotalPrice = (d.UnitPrice.HasValue && d.Quantity.HasValue)
                                ? (int)d.UnitPrice.Value * (int)d.Quantity.Value
                                : 0
                        }).ToList()
                }).ToList();

            // 課程訂單 + 細節
            var cOrders = _context.TCorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "潛水課程",
                    OrderId = o.OrderId,
                    OrderStatus = o.OrderStatus == true ? "已付款" : "Pending",
                    ShipAddress = null,
                    ShipPhone = null,
                    PaymentMethod = o.PaymentMethod,
                    OrderStatusId = null,
                    OrderLogId = null,
                    OrderDate = o.OrderDate,
                    SiteId = null,
                    Details = new List<OrderDetailDTO>
                    {
                        new OrderDetailDTO
                        {
                            ItemName = _context.TCcourses
                                .Where(c => c.CourseId == o.CourseId)
                                .Join(_context.TCcourseCategories,
                                    c => c.CourseCategoryId,
                                    cc => cc.CourseCategoryId,
                                    (c, cc) => cc.CategoryName)
                                .FirstOrDefault() ?? "未知課程",
                            Quantity = o.Quantity.HasValue ? (int)o.Quantity.Value : 0,
                            TotalPrice = (o.CoursePrice.HasValue && o.Quantity.HasValue)
                                ? (int)o.CoursePrice.Value * (int)o.Quantity.Value
                                : 0
                        }
                    }
                }).ToList();

            // 場地租借訂單
            var sOrders = _context.TSorders
                .Where(o => o.MemberId == memberId)
                .Select(o => new TMorderDTO
                {
                    OrderType = "場地租借",
                    OrderId = o.OrderId,
                    OrderStatus = o.OrderStatus,
                    ShipAddress = null,
                    ShipPhone = null,
                    PaymentMethod = o.PaymentMethod,
                    OrderStatusId = null,
                    OrderLogId = null,
                    OrderDate = null,
                    SiteId = o.SiteId,
                    Details = new List<OrderDetailDTO>
                    {
                        new OrderDetailDTO
                        {
                            ItemName = o.VenueName ?? "未知場地",
                            Quantity = 1,
                            TotalPrice = o.SitePay.HasValue ? (int)o.SitePay.Value : 0
                        }
                    }
                }).ToList();

            var allOrders = nOrders
                .Concat(uOrders)
                .Concat(cOrders)
                .Concat(sOrders)
                .OrderByDescending(o => o.OrderId)
                .ToList();

            return Ok(allOrders);
        }
    }
}