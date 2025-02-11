using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using Microsoft.CodeAnalysis;
using diveWebAPI.DTO;
using diveWebAPI.Partial;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNproductsController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TNproductsController(diveShopperContext context)
        {
            _context = context;
        }

        // GET /api/products/top?take=5
        [HttpGet("top")]
        public ActionResult<IEnumerable<TopProductDTO>> GetTopProducts([FromQuery] int take = 5)
        {
            // 從group ps by ps.ProductId 把同一商品聚合，在選擇要用 SUM、MAX 或其他聚合函數算出單一的 ViewCount
            // 再跟 TNproducts 做一次 join，最後 order by 你選出的統計值，Take(take) 取前 N 筆。
            var groupQuery =
       from ps in _context.ProductStats
       group ps by ps.ProductId into g
       select new
       {
           ProductId = g.Key,
           // 取 Sum
           TotalView = g.Sum(x => x.ViewCount)
       };

            var query =
                from g in groupQuery
                join p in _context.TNproducts on g.ProductId equals p.ProductId
                orderby g.TotalView descending
                select new TopProductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ImageUrl = p.ImageUrl,
                    UnitPrice = p.UnitPrice,
                    ViewCount = g.TotalView
                };

            var results = query.Take(take).ToList();
            return Ok(results);
        }


        // GET: api/TNproducts
        [HttpGet]
        public async Task<IEnumerable<TNproductDTO>> GetTNproducts([FromQuery] string? search)
        {
            // 先從資料庫取出產品的 IQueryable 物件
            var query = _context.TNproducts.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                // 可視需要，將 search 轉小寫或去除空白
                string keyword = search.Trim().ToLower();

                // 在這裡篩選 productName / Description
                // ToLower() 避免大小寫問題
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword)
                );
            }
            return query.Select(p=>new TNproductDTO {
            ProductId=p.ProductId,
            ProductName=p.ProductName,
            UnitPrice=p.UnitPrice,
            Description=p.Description,
             ImageUrl=p.ImageUrl

            });
        }

        // GET: api/TNproducts/5
        [HttpGet("{id}")]
        public async Task<TNproductDTO> GetTNproduct(int id)
        {
            var tNproduct = await _context.TNproducts.FindAsync(id);

            if (tNproduct == null)
            {
                return null;
            }
            TNproductDTO proDTO = new TNproductDTO
            {
                ProductId = tNproduct.ProductId,
                ProductName = tNproduct.ProductName,
                UnitPrice = tNproduct.UnitPrice,
                Description = tNproduct.Description,
                ImageUrl=tNproduct.ImageUrl

            };
            return proDTO;
        }

        // PUT: api/TNproducts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<string> PutTNproduct(int id, TNproductDTO proDTO)
        {
            if (id != proDTO.ProductId)
            {
                return "修改商品失敗";
            }
            TNproduct nproduct = await _context.TNproducts.FindAsync(id);
            nproduct.ProductName= proDTO.ProductName;
            nproduct.UnitPrice= proDTO.UnitPrice;
            nproduct.Description= proDTO.Description;
            nproduct.ImageUrl = proDTO.ImageUrl;
            _context.Entry(nproduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TNproductExists(id))
                {
                    return "修改商品失敗";
                }
                else
                {
                    throw;
                }
            }

            return "修改商品成功";
        }

        // POST: api/TNproducts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<TNproductDTO> PostTNproduct(TNproductDTO proDTO)
        {
            TNproduct nproduct = new TNproduct
            {
                ProductId = 0,
                UnitPrice = proDTO.UnitPrice,
                Description = proDTO.Description,
                ProductName = proDTO.ProductName,
                ImageUrl=proDTO.ImageUrl
            };
            _context.TNproducts.Add(nproduct);
            await _context.SaveChangesAsync();
            proDTO.ProductId=nproduct.ProductId;
            return proDTO;
        }

        // DELETE: api/TNproducts/5
        [HttpDelete("{id}")]
        public async Task<string> DeleteTNproduct(int id)
        {
            var tNproduct = await _context.TNproducts.FindAsync(id);
            if (tNproduct == null)
            {
                return "刪除商品失敗";
            }
            try
            {
                _context.TNproducts.Remove(tNproduct);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return "刪除商品失敗";
            }

            return "刪除商品成功";
        }

        private bool TNproductExists(int id)
        {
            return _context.TNproducts.Any(e => e.ProductId == id);
        }
    }
}
