using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.Partial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TMmemberListsAPIController : ControllerBase
    {
        private readonly diveShopperContext _context;

        public TMmemberListsAPIController(diveShopperContext context)
        {
            _context = context;
        }

        // GET: api/TMmemberListsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TMmemberList>>> GetTMmemberLists()
        {
            return await _context.TMmemberLists.ToListAsync();
        }

        // GET: api/TMmemberListsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TMmemberList>> GetTMmemberList(int id)
        {
            var tMmemberList = await _context.TMmemberLists.FindAsync(id);

            if (tMmemberList == null)
            {
                return NotFound();
            }

            return tMmemberList;
        }

        // PUT: api/TMmemberListsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTMmemberList(int id, TMmemberList tMmemberList)
        {
            if (id != tMmemberList.MemberId)
            {
                return BadRequest();
            }

            _context.Entry(tMmemberList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TMmemberListExists(id))
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

        // POST: api/TMmemberListsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TMmemberList>> PostTMmemberList(TMmemberList tMmemberList)
        {
            _context.TMmemberLists.Add(tMmemberList);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTMmemberList", new { id = tMmemberList.MemberId }, tMmemberList);
        }

        // DELETE: api/TMmemberListsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTMmemberList(int id)
        {
            var tMmemberList = await _context.TMmemberLists.FindAsync(id);
            if (tMmemberList == null)
            {
                return NotFound();
            }

            _context.TMmemberLists.Remove(tMmemberList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TMmemberListExists(int id)
        {
            return _context.TMmemberLists.Any(e => e.MemberId == id);
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
                token = token,
                memberId = user.MemberId
            });
        }
        private string GenerateJwtToken(TMmemberList user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aPj4eQm9TzGdK7xF5sLzN3vW8HcJ1dXq")); // 密鑰
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.MemberEmail),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("userId", user.MemberId.ToString()),
        new Claim("name", user.MemberName)
    };

            var token = new JwtSecurityToken(
                issuer: "diveShopper", // 發行者
                audience: "diveShopperClient", // 受眾
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId == null || !int.TryParse(userId, out int parsedUserId))
            {
                return Unauthorized(new { message = "無效的 Token，userId 不存在" });
            }

            var user = await _context.TMmemberLists.FindAsync(parsedUserId);
            if (user == null)
            {
                return NotFound(new { message = "找不到該使用者" });
            }

            string? base64Photo = user.MemberPhoto != null ? $"data:image/jpeg;base64,{Convert.ToBase64String(user.MemberPhoto)}" : null;

            return Ok(new
            {
                userId = parsedUserId,
                message = "成功獲取用戶資訊",
                user
            });
        }
    }
}
