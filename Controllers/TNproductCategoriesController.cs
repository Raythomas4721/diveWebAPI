using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.DTO;
using diveWebAPI.Partial;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TNproductCategoriesController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TNproductCategoriesController(diveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TNproductCategories
        [HttpGet]
        public async Task<IEnumerable<TNproductCategory>> GetTNproductCategories()
        {
            return await _context.TNproductCategories.ToListAsync();
        }

        // GET: api/TNproductCategories/5
        [HttpGet("{id}")]
        public async Task<TNcategoryDTO> GetTNproductCategory(int id)
        {
            var tNproductCategory = await _context.TNproductCategories.FindAsync(id);

            if (tNproductCategory == null)
            {
                return null;
            }
            TNcategoryDTO categoryDTO = new TNcategoryDTO
            {
                ProductCategoryId=tNproductCategory.ProductCategoryId,
                CategoryName=tNproductCategory.CategoryName,
                ParentCategoryId=tNproductCategory.ParentCategoryId,
               imageFileName=tNproductCategory.ImageFileName
            };
            return categoryDTO;
        }
        //取得該分類底下所有商品
        //GET: api/TNproductCategories/{id}/products
        [HttpGet("{id}/products")]
        public async Task<IEnumerable<TNproductDTO>> GetProductsByCategory(int id)
        {
            // 先在對應的 Mapping 表中，找出屬於此 CategoryId 的所有 productId
            var productIds = await _context.TNproductcategoryMappings
                .Where(m => m.ProductCategoryId == id)
                .Select(m => m.ProductId)
                .ToListAsync();

            // 再到 Products 裡面抓出這些 productId 的商品
            var products = await _context.TNproducts
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => new TNproductDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.UnitPrice,
                    Description = p.Description,
                    ImageUrl=p.ImageUrl
                    // ...若有更多欄位要回傳也可加上
                })
                .ToListAsync();

            return products;
        }

        // PUT: api/TNproductCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTNproductCategory(int id, TNproductCategory tNproductCategory)
        {
            if (id != tNproductCategory.ProductCategoryId)
            {
                return BadRequest();
            }

            _context.Entry(tNproductCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TNproductCategoryExists(id))
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

        // POST: api/TNproductCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TNproductCategory>> PostTNproductCategory(TNproductCategory tNproductCategory)
        {
            _context.TNproductCategories.Add(tNproductCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTNproductCategory", new { id = tNproductCategory.ProductCategoryId }, tNproductCategory);
        }

        // DELETE: api/TNproductCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTNproductCategory(int id)
        {
            var tNproductCategory = await _context.TNproductCategories.FindAsync(id);
            if (tNproductCategory == null)
            {
                return NotFound();
            }

            _context.TNproductCategories.Remove(tNproductCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TNproductCategoryExists(int id)
        {
            return _context.TNproductCategories.Any(e => e.ProductCategoryId == id);
        }
    }
}
