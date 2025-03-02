using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.DTO;
using diveWebAPI.Helper;
using System.Text;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNordersController : ControllerBase
    {
        private readonly DiveShopperContext _context;
        private readonly IConfiguration _config; // 新增

        public TNordersController(DiveShopperContext context, IConfiguration config)
        {
            _context = context;
            _config = config;

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
                OrderStatus = "Processing",  // 或預設 "New" / "Processing"
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
            var allCartItems = _context.TNcartItems
        .Where(ci => ci.MemberId == dto.MemberId);
            _context.TNcartItems.RemoveRange(allCartItems);
            await _context.SaveChangesAsync();
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

        [HttpGet("createECPayPayment/{orderId}")]
        public async Task<IActionResult> CreateECPayPayment(int orderId)
        {
            // 1) 確認該筆訂單是否存在
            var order = await _context.TNorders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found.");

            // 2) 產生 MerchantTradeNo（確保 <= 20字 & 僅英數）
            var merchantTradeNo = $"TN{orderId}{DateTime.Now:HHmmssfff}";

            // ★ 先把這個 MerchantTradeNo 存進 DB 的欄位
            order.MerchantTradeNo = merchantTradeNo;
            _context.TNorders.Update(order);
            await _context.SaveChangesAsync();  // 寫回資料庫

            // 2) 準備 ECPay 參數
            string serviceURL = _config["ECPay:ServiceURL"];
            string merchantID = _config["ECPay:MerchantID"];
            string hashKey = _config["ECPay:HashKey"];
            string hashIV = _config["ECPay:HashIV"];
            string returnURL = _config["ECPay:ReturnURL"];       // 後端 Callback
            string resultURL = _config["ECPay:ClientBackURL"];  // 前端顯示結果

            // 建議自訂的交易編號 (MerchantTradeNo) 確保唯一
            // 例如： "Order202302011530_123" or "TNorder_{orderId}_{ticks}"

            var merchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string ecpayResult = "http://localhost:4200/#/ecpayResult";
            // 對應綠界最基本的欄位 (更多欄位請參考官方文件)
            var parameters = new Dictionary<string, string>
    {
        { "MerchantID", merchantID },
        { "MerchantTradeNo", merchantTradeNo },
        { "MerchantTradeDate", merchantTradeDate },
        { "PaymentType", "aio" },
        { "TotalAmount", ((int)order.TotalAmount).ToString() },   // 訂單金額
        { "TradeDesc", "DiveShopperOrder" },              // 交易描述
        { "ItemName", "1" },                         // 例如"ABC商品 x1"
        { "ReturnURL", returnURL },                        // 綠界背景通知
        { "ClientBackURL",  $"{ecpayResult}?orderId={orderId}" },                   // 付款後前端導頁
        { "ChoosePayment", "Credit" },                        // 支援全部支付方式
        { "EncryptType", "1" }
    };

            // 3) 產生 CheckMacValue
            var checkMacValue = CheckMacValue.GenerateCheckMacValue(parameters, hashKey, hashIV);
            parameters.Add("CheckMacValue", checkMacValue);
            // ★ log: 印出「最終要送給綠界的全部參數」
            Console.WriteLine("=== Final parameters to ECPay ===");
            foreach (var kv in parameters)
            {
                Console.WriteLine($"{kv.Key} = {kv.Value}");
            }
            // 4) 產生自動提交的 HTML Form (示範)
            var htmlForm = BuildECPayAutoSubmitForm(serviceURL, parameters);

            // 5) 回傳給前端：前端瀏覽器會自動導頁到綠界測試站
            return Content(htmlForm, "text/html");
        }

        // 工具函式：組成自動提交的表單
        private string BuildECPayAutoSubmitForm(string actionUrl, Dictionary<string, string> fields)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<form id='ecpayForm' method='POST' action='{actionUrl}'>");
            foreach (var kv in fields)
            {
                sb.AppendLine($"<input type='hidden' name='{kv.Key}' value='{kv.Value}' />");
                Console.WriteLine($"[FORM] {kv.Key} = {kv.Value}");
            }
            sb.AppendLine("</form>");
            // 下方改成引用外部檔案 (需確保 /js/ecpayAutoSubmit.js 存在)
            sb.AppendLine("<script src='/js/ecpayAutoSubmit.js'></script>");

            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        [HttpPost("ECPayCallback")]
        public async Task<IActionResult> ECPayCallback([FromForm] Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
                return Ok(); // 或 BadRequest()

            // 1) 讀取 HashKey / HashIV
            string hashKey = _config["ECPay:HashKey"];
            string hashIV = _config["ECPay:HashIV"];

            // 2) 驗證 CheckMacValue
            bool isValid = CheckMacValue.ValidateCheckMacValue(formData, hashKey, hashIV);
            if (!isValid)
            {
                // 驗證失敗 => 可能被竄改
                return Content("0|Fail");
            }

            // 3) 取出綠界回傳的重要參數
            //    RtnCode == 1 => 付款成功
            var rtnCode = formData.ContainsKey("RtnCode") ? formData["RtnCode"] : "";
            var merchantTradeNo = formData.ContainsKey("MerchantTradeNo") ? formData["MerchantTradeNo"] : "";
            var tradeNo = formData.ContainsKey("TradeNo") ? formData["TradeNo"] : "";   // 綠界交易編號
            var tradeDate = formData.ContainsKey("PaymentDate") ? formData["PaymentDate"] : ""; // 交易完成時間

            // 4) 依據自訂 MerchantTradeNo 找到對應的訂單 (可能要從 MerchantTradeNo 解析出對應 orderId)
            //    如果你在 merchantTradeNo 的命名中有包含 orderId，可在這裡做字串切割/解析
            //    例如 merchantTradeNo = "TN_123_20230206120100001"
            //    就能 parse 出原本的 orderId = 123
            // ★ 直接用 MerchantTradeNo 找訂單
            var order = await _context.TNorders
                .FirstOrDefaultAsync(o => o.MerchantTradeNo == merchantTradeNo);

            if (order == null)
                return Content("0|Fail");

            // 4) 判斷是否付款成功
            if (rtnCode == "1")
            {
                order.OrderStatus = "Paid";
                order.PaymentDate = DateTime.Now;
                order.PaymentMethod = "ECPay";
                // 也可以記錄 tradeNo 等
            }
            else
            {
                order.OrderStatus = $"Payment_Failed({rtnCode})";
            }

            await _context.SaveChangesAsync();

            return Content("1|OK");
        }
    
}
}
