using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.DTO;
using Humanizer;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNreviewsController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TNreviewsController(diveShopperContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TNreviewDTO>> PostTNreview(TNreviewDTO reviewDto)
        {
            // 1) 先檢查該 memberId + productId 是否曾在訂單明細出現 & 狀態是已完成/已付款
            bool hasPurchased = await _context.TNorderDetails
                .Include(od => od.Order)              // 連到 TNorder
                .Include(od => od.Productvariants)    // 連到 TNproductvariant
                .AnyAsync(od =>
                     od.Order.MemberId == reviewDto.MemberId                 // 這筆訂單是該會員的
                  && od.Productvariants.ProductId == reviewDto.ProductId     // 該訂單明細對應的商品ID
                  && (od.Order.OrderStatus == "Completed" || od.Order.OrderStatus == "Pending")
                );

            if (!hasPurchased)
            {
                // 沒買過 → 回傳 403 Forbidden
                return StatusCode(StatusCodes.Status403Forbidden, "You have not purchased this product, so you cannot leave a review.");
            }
            //檢查避免重複新增
            bool exist = await _context.TNreviews.AnyAsync(r =>
            r.MemberId == reviewDto.MemberId && r.ProductId == reviewDto.ProductId
);
            if (exist) return BadRequest("You already wrote a review for this product.");

            // 2) 建立 TNreview 實體 (EF 的資料表對應)
            var reviewEntity = new TNreview
            {
                MemberId = reviewDto.MemberId,
                ProductId = reviewDto.ProductId,
                MemberName = reviewDto.MemberName,
                ReviewRating = reviewDto.ReviewRating,
                ReviewContent = reviewDto.ReviewContent,
                CreatedDate = DateTime.Now  // or reviewDto.CreatedDate
            };

            // 3) 新增到資料庫
            _context.TNreviews.Add(reviewEntity);
            await _context.SaveChangesAsync();

            // 4) 把新增後的資訊寫回 DTO (包含主鍵 ReviewId)
            reviewDto.ReviewId = reviewEntity.ReviewId;
            reviewDto.CreatedDate = reviewEntity.CreatedDate;

            // 5) 回傳 201 Created + 新增後的 DTO
            return CreatedAtAction(nameof(GetTNreview), new { id = reviewDto.ReviewId }, reviewDto);
        }

        // 範例: GET 單筆 review
        [HttpGet("{id}")]
        public async Task<ActionResult<TNreviewDTO>> GetTNreview(int id)
        {
            var review = await _context.TNreviews.FindAsync(id);
            if (review == null)
                return NotFound();

            // 轉成 DTO
            var dto = new TNreviewDTO
            {
                ReviewId = review.ReviewId,
                MemberId = review.MemberId,
                ProductId = review.ProductId,
                MemberName = review.MemberName,
                ReviewRating = review.ReviewRating,
                ReviewContent = review.ReviewContent,
                CreatedDate = review.CreatedDate
            };

            return dto;
        }

        // GET: api/TNreviews/product/5
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<TNreviewDTO>>> GetReviewsByProduct(int productId)
        {
            // 1) 從資料庫查詢符合 productId 的所有評論
            var reviewEntities = await _context.TNreviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            // 2) 如果該商品還沒任何評論，可以回傳空陣列，也可做其他處理
            // if (!reviewEntities.Any()) return NotFound("這個商品還沒有任何評論");

            // 3) 轉成 DTO
            var dtos = reviewEntities.Select(r => new TNreviewDTO
            {
                ReviewId = r.ReviewId,
                MemberId = r.MemberId,
                ProductId = r.ProductId,
                MemberName = r.MemberName,
                ReviewRating = r.ReviewRating,
                ReviewContent = r.ReviewContent,
                CreatedDate = r.CreatedDate
            }).ToList();

            // 4) 回傳該商品的全部評論
            return dtos;
        }
        [HttpPut("{reviewId}")]
        public async Task<ActionResult> UpdateReview(int reviewId, TNreviewDTO dto)
        {
            // 1) 找出資料
            var review = await _context.TNreviews.FindAsync(reviewId);
            if (review == null)
                return NotFound("Review not found");

            // 2) 檢查擁有者
            if (review.MemberId != dto.MemberId)
                return StatusCode(403, "You can't edit others' review");

            // 3) 更新
            review.ReviewRating = dto.ReviewRating;
            review.ReviewContent = dto.ReviewContent;
            review.UpdateDate = DateTime.Now; // 你可自行新增 UpdatedDate 欄位

            // 4) Save
            await _context.SaveChangesAsync();
            return Ok(new { message = "Review updated" });
        }

        [HttpDelete("{reviewId}")]
        public async Task<ActionResult> DeleteReview(int reviewId, [FromBody] TNreviewDTO dto)
        {
            // 1) 找出資料
            var review = await _context.TNreviews.FindAsync(reviewId);
            if (review == null)
                return NotFound("Review not found");

            // 2) 檢查擁有者
            if (review.MemberId != dto.MemberId)
                return StatusCode(StatusCodes.Status403Forbidden, "You can't edit others' review");

            _context.TNreviews.Remove(review); 
            await _context.SaveChangesAsync();
            return Ok(new { message = "Review updated" });
        }



        private bool TNreviewExists(int id)
        {
            return _context.TNreviews.Any(e => e.ReviewId == id);
        }
    }
}
