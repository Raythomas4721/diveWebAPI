using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using diveWebAPI.DTO;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TCcoursesController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TCcoursesController(DiveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TCcourses
        [HttpGet]
        public async Task<IEnumerable<TCcourseDTO>> GetTCcourses()
        {
            return _context.TCcourses
                .Include(e => e.CourseCategory)
                .Include(e => e.Level)
                .Include(e => e.Coach)
                .Select(e=>new TCcourseDTO {
                CourseId=e.CourseId,
                CategoryName = e.CourseCategory.CategoryName,
                Description = e.CourseCategory.Description, // 新增這行
                LevelName = e.Level.LevelName,
                CoachName = e.Coach.CoachName,
                CoursePrice=e.CoursePrice,
                Photo=e.Photo,
                CreatedAt=e.CreatedAt,
                UpdatedAt=e.UpdatedAt,
                Discription=e.Discription,
                CourseStatus=e.CourseStatus,
                StartAt = e.StartAt
            });
        }

        // GET: api/TCcourses/5
        [HttpGet("{id}")]
        public async Task<TCcourseDTO> GetTCcourse(int id)
        {
            var tCcourse = _context.TCcourses
                .Include(e => e.CourseCategory)
                .Include(e => e.Level)
                .Include(e => e.Coach)
                .FirstOrDefault(e => e.CourseId == id);

            if (tCcourse == null)
            {
                return null;
            }
            TCcourseDTO courseDTO = new TCcourseDTO
            {
                CourseId = tCcourse.CourseId,
                CategoryName = tCcourse.CourseCategory.CategoryName,
                LevelName = tCcourse.Level.LevelName,
                CoachName = tCcourse.Coach.CoachName,
                CoursePrice = tCcourse.CoursePrice,
                Photo = tCcourse.Photo,
                CreatedAt = tCcourse.CreatedAt,
                UpdatedAt = tCcourse.UpdatedAt,
                Discription = tCcourse.Discription,
                CourseStatus = tCcourse.CourseStatus,
                StartAt = tCcourse.StartAt

            };
            return courseDTO;
        }

        // PUT: api/TCcourses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<String> PutTCcourse(int id, TCcourseDTO courseDTO)
        {
            if (id != courseDTO.CourseId)
            {
                return "修改課程失敗";
            }
            int categoryId = _context.TCcourseCategories.FirstOrDefault(e => e.CategoryName == courseDTO.CategoryName).CourseCategoryId;
            int levelId = _context.TCcourseLevels.FirstOrDefault(e => e.LevelName == courseDTO.LevelName).LevelId;
            int coachId = _context.TMcoaches.FirstOrDefault(e => e.CoachName == courseDTO.CoachName).CoachId;
            TCcourse tCcourse = await _context.TCcourses.FindAsync(id);
            tCcourse.CourseCategoryId = categoryId;
            tCcourse.LevelId = levelId;
            tCcourse.CoachId = coachId;
            tCcourse.CoursePrice = courseDTO.CoursePrice;
            tCcourse.Photo = courseDTO.Photo;
            tCcourse.UpdatedAt = DateTime.Now; 
            tCcourse.Discription= courseDTO.Discription;
            tCcourse.CourseStatus= courseDTO.CourseStatus;
            tCcourse.StartAt= courseDTO.StartAt;
           

            try
            {
                _context.TCcourses.Update(tCcourse);
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TCcourseExists(id))
                {
                    return "修改課程資料庫失敗";
                }
                else
                {
                    throw;
                }
            }

            return "修改課程成功";
        }

        // POST: api/TCcourses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<String> PostTCcourse(TCcourseDTO courseDTO)
        {
            int levelId = _context.TCcourseLevels.FirstOrDefault(e => e.LevelName == courseDTO.LevelName).LevelId;
            int coachId = _context.TMcoaches.FirstOrDefault(e => e.CoachName == courseDTO.CoachName).CoachId;
            int categoryId = _context.TCcourseCategories.FirstOrDefault(e => e.CategoryName == courseDTO.CategoryName).CourseCategoryId;
            TCcourse tccourse = new TCcourse {
                CourseId=0,
                LevelId = levelId,
                CoachId = coachId,
                CoursePrice = courseDTO.CoursePrice,
                Photo = courseDTO.Photo,
                CourseCategoryId = categoryId,
                UpdatedAt = DateTime.Now,
                Discription= courseDTO.Discription,
                CourseStatus= courseDTO.CourseStatus,
                StartAt= courseDTO.StartAt

            };
            _context.TCcourses.Add(tccourse);
            await _context.SaveChangesAsync();
            return $"課程編號:{tccourse.CourseId}";

          
        }

        // DELETE: api/TCcourses/5
        [HttpDelete("{id}")]
        public async Task<String> DeleteTCcourse(int id)
        {
            var tCcourse = await _context.TCcourses.FindAsync(id);
            if (tCcourse == null)
            {
                return "刪除課程失敗";
            }
            try
            {
                _context.TCcourses.Remove(tCcourse);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) 
            {
                return "刪除課程關聯記錄失敗";
            }
            

            return "刪除成功";
        }

        private bool TCcourseExists(int id)
        {
            return _context.TCcourses.Any(e => e.CourseId == id);
        }

        [HttpGet("categories")]
        public async Task<IEnumerable<string>> GetCourseCategories()
        {
            return await _context.TCcourseCategories
                .Select(c => c.CategoryName)
                .ToListAsync();
        }
    }
}
