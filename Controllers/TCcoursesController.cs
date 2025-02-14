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
            return _context.TCcourses.Select(e=>new TCcourseDTO {
                CourseId=e.CourseId,
                CourseCategoryId=e.CourseCategoryId,
                LevelId=e.LevelId,
                CoachId=e.CoachId,
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
            var tCcourse = await _context.TCcourses.FindAsync(id);

            if (tCcourse == null)
            {
                return null;
            }
            TCcourseDTO courseDTO = new TCcourseDTO
            {
                CourseId = tCcourse.CourseId,
                CourseCategoryId = tCcourse.CourseCategoryId,
                LevelId = tCcourse.LevelId,
                CoachId = tCcourse.CoachId,
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
            TCcourse tCcourse = await _context.TCcourses.FindAsync(id);
            tCcourse.CourseCategoryId=courseDTO.CourseCategoryId;
            tCcourse.LevelId=courseDTO.LevelId;
            tCcourse.CoachId=courseDTO.CoachId;
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
            TCcourse tccourse = new TCcourse {
                CourseId=0,
                LevelId=courseDTO.LevelId,
                CoachId=courseDTO.CoachId,
                CoursePrice = courseDTO.CoursePrice,
                Photo = courseDTO.Photo,
                CourseCategoryId=courseDTO.CourseCategoryId,
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
    }
}
