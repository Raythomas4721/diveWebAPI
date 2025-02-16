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
                  && (od.Order.OrderStatus == "Completed" || od.Order.OrderStatus == "Paid")
                );

            if (!hasPurchased)
            {
                // 沒買過 → 回傳 403 Forbidden
                return StatusCode(StatusCodes.Status403Forbidden, "You have not purchased this product, so you cannot leave a review.");
            }

            // 2) 建立 TNreview 實體 (EF 的資料表對應)
            var reviewEntity = new TNreview
            {
                MemberId = reviewDto.MemberId,
                ProductId = reviewDto.ProductId,
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
            return CreatedAtAction(nameof(GetTNreview), new { id = reviewEntity.ReviewId }, reviewDto);
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
                ReviewRating = review.ReviewRating,
                ReviewContent = review.ReviewContent,
                CreatedDate = review.CreatedDate
            };

            return dto;
        }
    
        private bool TNreviewExists(int id)
        {
            return _context.TNreviews.Any(e => e.ReviewId == id);
        }
    }
}
