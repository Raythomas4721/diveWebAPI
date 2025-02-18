using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Azure.Core;
using Microsoft.AspNetCore.Cors;

namespace diveWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TMmemberListsAPIController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public TMmemberListsAPIController(DiveShopperContext context)
        {
            _context = context;
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var userData = await _context.TMmemberLists
                .Where(u => u.MemberEmail == loginRequest.Email)
                .Select(u => new
                {
                    u.MemberId,
                    u.MemberEmail,
                    u.MemberName,
                    u.MemberPassword,
                    u.RecentLogin
                }).FirstOrDefaultAsync();

            if (userData == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, userData.MemberPassword))
            {
                return Unauthorized(new { status = false, message = "帳號或密碼錯誤" });
            }

            var user = await _context.TMmemberLists.FirstOrDefaultAsync(u => u.MemberEmail == loginRequest.Email);
            if (user != null)
            {
                user.RecentLogin = DateTime.UtcNow;
                _context.TMmemberLists.Update(user);
                await _context.SaveChangesAsync();
            }

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                status = true,
                message = "登入成功",
                token = token
            });
        }
        private string GenerateJwtToken(TMmemberList user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aPj4eQm9TzGdK7xF5sLzN3vW8HcJ1dXq")); 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userId", user.MemberId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.MemberEmail),
                new Claim("name", user.MemberName)
            };

            var token = new JwtSecurityToken(
                issuer: "diveShopper",
                audience: "diveShopperClient",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegisterRequest
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (_context.TMmemberLists.Any(u => u.MemberEmail == request.Email))
            {
                return BadRequest(new { status = false, message = "該 Email 已經註冊" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password); // 進行密碼加密

            var newUser = new TMmemberList
            {
                MemberName = request.Name,
                MemberEmail = request.Email,
                MemberPassword = hashedPassword,
                RecentLogin = DateTime.UtcNow,
                Status = true,
            };

            _context.TMmemberLists.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "註冊成功" });
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userIdClaim = User.FindFirst("userId")?.Value; 

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int parsedUserId))
            {
                Console.WriteLine(" Token 解析 `userId` 失敗！");
                return Unauthorized(new { message = "無效的 Token，解析 `userId` 失敗" });
            }

            Console.WriteLine($"解析 `userId`: {parsedUserId}");

            var user = await _context.TMmemberLists.FindAsync(parsedUserId);
            if (user == null)
            {
                Console.WriteLine($"找不到該使用者 `userId`: {parsedUserId}");
                return NotFound(new { message = "找不到該使用者" });
            }

            Console.WriteLine("成功獲取用戶資訊，返回資料");

            return Ok(new
            {
                memberId = parsedUserId,
                message = "成功獲取用戶資訊",
                user
            });
        }

        public class EditUserInfo
        {
            public string? MemberName { get; set; }
            public string? MemberPhone { get; set; }
            public string? MemberAddress { get; set; }
            public string? UrgentContact { get; set; }
            public string? UrgentPhone { get; set; }

        }
        
        [Authorize]
        [HttpPatch("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] EditUserInfo request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (!int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized(new { message = "無效的 Token，userId 不存在" });
            }
            var user = await _context.TMmemberLists.FindAsync(parsedUserId);
            if (user == null)
            {
                return NotFound(new { Message = "找不到該使用者" });
            }

            user.MemberName = request.MemberName;
            user.MemberPhone = request.MemberPhone;
            user.MemberAddress = request.MemberAddress;
            user.UrgentContact = request.UrgentContact;
            user.UrgentPhone = request.UrgentPhone;
            user.Status = true;


            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { status = true, message = "使用者資訊變更成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "更新使用者資訊時發生錯誤", error = ex.Message });
            };
        }
        [Authorize]
        [HttpPut("ChangeUserPhoto")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public async Task<IActionResult> ChangeUserPhoto([FromForm] IFormFile photo)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId == null || !int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized(new { message = "無效的 Token，userId 不存在" });
            }

            var user = await _context.TMmemberLists.FindAsync(parsedUserId);
            if (user == null)
            {
                return NotFound(new { message = "找不到該使用者，請確認 userId 是否有效" });
            }

            if (photo == null || photo.Length == 0)
            {
                return BadRequest(new { message = "未提供圖片" });
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    byte[] newPhotoBytes = memoryStream.ToArray();

                    // 計算 Hash 確保圖片不重複
                    string newPhotoHash = Convert.ToBase64String(SHA256.HashData(newPhotoBytes));
                    string currentPhotoHash = user.MemberPhoto != null ? Convert.ToBase64String(SHA256.HashData(user.MemberPhoto)) : "";

                    if (newPhotoHash == currentPhotoHash)
                    {
                        return BadRequest(new { message = "這張圖片正被你使用中！" });
                    }

                    user.MemberPhoto = newPhotoBytes;
                }

                await _context.SaveChangesAsync();

                string contentType = string.IsNullOrEmpty(photo.ContentType) ? "image/jpeg" : photo.ContentType;
                string base64Photo = $"data:{contentType};base64,{Convert.ToBase64String(user.MemberPhoto)}";

                return Ok(new { status = true, message = "圖片上傳成功", memberPhoto = base64Photo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "圖片上傳失敗", error = ex.Message });
            }
        }
        public class ChangePasswordDTO
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }

        }
        [Authorize]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId == null || !int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized(new { message = "無效的 Token" });
            }

            var user = await _context.TMmemberLists.FindAsync(parsedUserId);
            if (user == null)
            {
                return NotFound(new { message = "找不到該使用者" });
            }

            // 確認舊密碼
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.MemberPassword))
            {
                return BadRequest(new { message = "舊密碼錯誤" });
            }

            // 更新新密碼
            user.MemberPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "密碼更新成功" });
        }





        // GET: api/TMmemberListsAPI
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TMmemberList>>> GetTMmemberLists()
        //{
        //    return await _context.TMmemberLists.ToListAsync();
        //}

        //// GET: api/TMmemberListsAPI/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TMmemberList>> GetTMmemberList(int id)
        //{
        //    var tMmemberList = await _context.TMmemberLists.FindAsync(id);

        //    if (tMmemberList == null)
        //    {
        //        return NotFound();
        //    }

        //    return tMmemberList;
        //}

        //// PUT: api/TMmemberListsAPI/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTMmemberList(int id, TMmemberList tMmemberList)
        //{
        //    if (id != tMmemberList.MemberId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(tMmemberList).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TMmemberListExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/TMmemberListsAPI
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<TMmemberList>> PostTMmemberList(TMmemberList tMmemberList)
        //{
        //    _context.TMmemberLists.Add(tMmemberList);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetTMmemberList", new { id = tMmemberList.MemberId }, tMmemberList);
        //}

        //// DELETE: api/TMmemberListsAPI/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTMmemberList(int id)
        //{
        //    var tMmemberList = await _context.TMmemberLists.FindAsync(id);
        //    if (tMmemberList == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.TMmemberLists.Remove(tMmemberList);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool TMmemberListExists(int id)
        {
            return _context.TMmemberLists.Any(e => e.MemberId == id);
        }
    }
}
