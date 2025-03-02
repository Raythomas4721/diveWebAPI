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
    public class TNdiscountsController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TNdiscountsController(DiveShopperContext context)
        {
            _context = context;
        }



        [HttpPost("ByProducts")]
        public async Task<ActionResult<IEnumerable<ProductDiscountResult>>> GetDiscountByProducts([FromBody] int[] productIds)
        {
            // 1) 若沒傳入任何 productIds
            if (productIds == null || productIds.Length == 0)
            {
                return BadRequest("No productIds provided.");
            }

            var today = DateTime.Today;

            // 2) 在 Mappings 表中，一次查出所有和這些 productIds 有關的紀錄
            //    => group by productId
            var mappings = await _context.TNproductcategoryMappings
                .Where(m => productIds.Contains(m.ProductId.Value))
                .Select(m => new { m.ProductId, m.ProductCategoryId })
                .ToListAsync();

            // 3) 找出這些 productId 對應的所有 categoryIds (可能重複)
            var allCategoryIds = mappings
                .Where(x => x.ProductCategoryId.HasValue)
                .Select(x => x.ProductCategoryId.Value)
                .Distinct()
                .ToList();

            if (!allCategoryIds.Any())
            {
                // 代表這些商品都沒有對應任何分類
                // 可直接回傳 [ { productId=..., discountValue=null }, ... ]
                var emptyResult = productIds.Select(pid =>
                    new ProductDiscountResult { ProductId = pid, DiscountValue = null });
                return Ok(emptyResult);
            }

            // 4) 一次查詢所有分類ID在 allCategoryIds 之內、且日期有效的折扣
            var validDiscounts = await _context.TNdiscounts
                .Where(d => d.ProductCategoryId.HasValue
                            && allCategoryIds.Contains(d.ProductCategoryId.Value))
                .Where(d => d.StartDate <= today && d.EndDate >= today)
                .Select(d => new {
                    CategoryId = d.ProductCategoryId.Value,
                    DiscountValue = d.DiscountValue
                })
                .ToListAsync();
            // 現在 validDiscounts 內是 [ {CategoryId=9, DiscountValue=80}, {CategoryId=3, ...}, ... ]

            // 5) 建立一個 Dictionary<productId, bestDiscountValue> 做累積
            var productBestDiscountMap = new Dictionary<int, decimal?>();

            // 6) 對每個 productId
            //    先找出它的所有 categoryId
            //    再從 validDiscounts 裡找出同樣 categoryId 的折扣
            //    取最大 DiscountValue
            foreach (var pid in productIds)
            {
                // 找出該 productId 所有分類
                var catIdsForThisProduct = mappings
                    .Where(m => m.ProductId == pid)
                    .Select(m => m.ProductCategoryId.Value)
                    .Distinct();

                // 從 validDiscounts 找出屬於 catIdsForThisProduct 的折扣
                var discountsForThisProduct = validDiscounts
                    .Where(vd => catIdsForThisProduct.Contains(vd.CategoryId))
                    .Select(vd => vd.DiscountValue ?? 100M);

                if (!discountsForThisProduct.Any())
                {
                    // 沒找到有效折扣 => discountValue = null
                    productBestDiscountMap[pid] = null;
                }
                else
                {
                    // 取最大 discountValue 
                    // (假設 80=8折, 90=9折 => 90 > 80 => 9折?)
                    var best = discountsForThisProduct.Max();
                    productBestDiscountMap[pid] = best;
                }
            }

            // 7) 轉成 List<ProductDiscountResult>
            var result = productBestDiscountMap
                .Select(kv => new ProductDiscountResult
                {
                    ProductId = kv.Key,
                    DiscountValue = kv.Value
                })
                .ToList();

            return Ok(result);
        }


        // GET: api/Discounts
        // 回傳「目前所有有效的折扣」，並轉成 TNdiscountDTO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TNdiscountDTO>>> GetCurrentDiscounts()
        {
            var today = DateTime.Now.Date;

            // 1) 只撈 StartDate <= 今天 && EndDate >= 今天 的折扣
            // 2) 用 Select(...) 把資料庫的 TNdiscount 實體轉成 TNdiscountDTO
            var discounts = await _context.TNdiscounts
                .Where(d => d.StartDate <= today && d.EndDate >= today)
                .Select(d => new TNdiscountDTO
                {
                    DiscountId = d.DiscountId,
                    DiscountName = d.DiscountName,
                    ProductCategoryId = d.ProductCategoryId,
                    DiscountValue = d.DiscountValue,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate
                })
                .ToListAsync();

            return Ok(discounts);
        }

        // GET: api/Discounts/{categoryId}
        // 回傳「對某個分類ID」目前生效的折扣(若有多筆，也可自行取最大折扣或全部回傳)
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<TNdiscountDTO>> GetDiscountByCategory(int categoryId)
        {
            var today = DateTime.Now.Date;

            // 同樣用 Select(...) 投影成 DTO
            var discount = await _context.TNdiscounts
                .Where(d => d.ProductCategoryId == categoryId)
                .Where(d => d.StartDate <= today && d.EndDate >= today)
                .Select(d => new TNdiscountDTO
                {
                    DiscountId = d.DiscountId,
                    DiscountName = d.DiscountName,
                    ProductCategoryId = d.ProductCategoryId,
                    DiscountValue = d.DiscountValue,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate
                })
                .FirstOrDefaultAsync();

            if (discount == null)
            {
                return NotFound();
            }

            return Ok(discount);
        }
    }
}
