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
            var user = await _context.TMmemberLists.FirstOrDefaultAsync(u => u.MemberEmail == loginRequest.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.MemberPassword))
            {
                return Unauthorized(new { status = false, message = "帳號或密碼錯誤" });
            }
            user.RecentLogin = DateTime.UtcNow;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new { status = true, message = "登入成功", token = token, user = new { user.MemberId, user.MemberEmail } });
        }
        private string GenerateJwtToken(TMmemberList user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aPj4eQm9TzGdK7xF5sLzN3vW8HcJ1dXq")); // 密鑰
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.MemberEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.MemberId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "diveShopper", // 發行者
                audience: "diveShopperClient", // 受眾
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Token 有效期
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
        public IActionResult GetUserProfile()
        {
            Console.WriteLine("進入 GetUserProfile 方法"); 

            // 檢查 Token 是否解析出 User.Claims
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            Console.WriteLine($"Claims: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (userId == null)
            {
                Console.WriteLine("無效的 Token，userId 為空");
                return Unauthorized(new { message = "無效的 Token" });
            }

            return Ok(new { userId, message = "成功獲取用戶資訊" });
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
    }
}
