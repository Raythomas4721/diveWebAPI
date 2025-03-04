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
using diveWebAPI.DTO;
using Org.BouncyCastle.Asn1.Ocsp;
using diveWebAPI.Services;

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
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO loginRequest, [FromServices] IEmailService emailService)
        {
            var email = loginRequest.Email?.Trim().ToLower();

            var user = await _context.TMmemberLists
                .FirstOrDefaultAsync(u => u.MemberEmail == email);

            if (user == null)
            {
                return BadRequest(new { status = false, message = "此電子郵件尚未註冊，請確認拼寫是否正確，或註冊一個新帳號。" });
            }

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var remainingTime = (user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes;
                return Unauthorized(new { status = false, message = $"帳戶因多次失敗暫時鎖定，請在 {(int)remainingTime} 分鐘後再試，或立即重置密碼。" });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.MemberPassword))
            {
                user.LoginAttempts = user.LoginAttempts.HasValue ? user.LoginAttempts.Value + 1 : 1;

                if (user.LoginAttempts >= 4)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(10); // 鎖定10分鐘

                    // 生成重置密碼 Token
                    var resetToken = Guid.NewGuid().ToString(); // 簡單的唯一識別碼
                    user.ResetPasswordToken = resetToken;
                    user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddMinutes(10); // Token 有效期10分鐘

                    _context.TMmemberLists.Update(user);
                    await _context.SaveChangesAsync();

                    var subject = "登入失敗通知";
                    var body = $@"
                <h2>帳戶安全通知</h2>
                <p>親愛的使用者，您好：</p>
                <p>您的帳戶（{user.MemberEmail}）已連續登入失敗超過3次，請問是否為本人操作？</p>
                <p>帳戶現已暫時鎖定10分鐘。請點擊<a href='http://localhost:4200/#/reset-password?token={resetToken}'>此連結重置密碼</a>，或等待10分鐘後再試。</p>
                <p>若非本人操作，請盡快與我們聯繫。</p>
                <p>潛力無限 DiveX 團隊</p>";

                    try
                    {
                        await emailService.SendEmailAsync(user.MemberEmail, subject, body);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send email: {ex.Message}");
                    }

                    return Unauthorized(new { status = false, message = "密碼錯誤次數過多，帳戶已暫時鎖定10分鐘，已發送通知至您的電子郵件，請檢查並重置密碼。" });
                }

                _context.TMmemberLists.Update(user);
                await _context.SaveChangesAsync();
                return Unauthorized(new { status = false, message = $"帳號或密碼錯誤，您還有 {3 - user.LoginAttempts} 次嘗試機會" });
            }

            user.LoginAttempts = 0;
            user.LockoutEnd = null;
            user.ResetPasswordToken = null; // 清除 Token
            user.ResetPasswordTokenExpiry = null;
            user.RecentLogin = DateTime.UtcNow;
            _context.TMmemberLists.Update(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new { status = true, message = "登入成功", token = token });
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO resetRequest)
        {
            // 驗證 Token 是否有效
            var user = await _context.TMmemberLists
                .FirstOrDefaultAsync(u => u.ResetPasswordToken == resetRequest.Token);

            if (user == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest(new { status = false, message = "無效或過期的重置密碼連結，請重新申請。" });
            }

            // 更新密碼並清除 Token
            user.MemberPassword = BCrypt.Net.BCrypt.HashPassword(resetRequest.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;
            user.LockoutEnd = null; // 解除鎖定
            user.LoginAttempts = 0; // 重置失敗次數

            _context.TMmemberLists.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "密碼已成功重置，請使用新密碼登入。" });
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
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
        [Authorize]
        [HttpPut("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo([FromBody] EditUserInfoDTO request)
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
            if (string.IsNullOrWhiteSpace(request.MemberName))
            {
                return BadRequest(new { message = "會員姓名不能為空" });
            }

            if (request.MemberName.Length > 40) 
            {
                return BadRequest(new { message = "會員姓名長度不能超過40個字符" });
            }

            if (!string.IsNullOrWhiteSpace(request.MemberPhone))
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(request.MemberPhone, @"^\d{10}$"))
                {
                    return BadRequest(new { message = "電話號碼格式不正確，應為10位數字" });
                }
            }

            user.MemberName = request.MemberName.Trim();
            user.MemberPhone = request.MemberPhone;
            user.MemberAddress = request.MemberAddress.Trim();
            user.UrgentContact = request.UrgentContact.Trim();
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

        private bool TMmemberListExists(int id)
        {
            return _context.TMmemberLists.Any(e => e.MemberId == id);
        }
    }
}
