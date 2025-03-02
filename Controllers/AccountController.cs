using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using diveWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using diveWebAPI.Services; // 假設 IEmailService 在這個命名空間中
using System.Net.Http;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DiveShopperContext _context;
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;
        // 臨時儲存驗證碼（建議生產環境使用 Redis 或資料庫）
        private static readonly Dictionary<string, (string Code, DateTime ExpiryTime)> _verificationCodes = new();
        // 儲存每個用戶的驗證請求時間戳
        private static readonly Dictionary<string, List<DateTime>> _verificationAttempts = new();

        public AccountController(DiveShopperContext context, IEmailService emailService)
        {
            _context = context;
            _httpClient = new HttpClient();
            _emailService = emailService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authResult.Succeeded)
            {
                return Unauthorized("Google 驗證失敗");
            }

            var email = authResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var profilePicUrl = authResult.Principal.FindFirst("picture")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("無法獲取 Google 用戶資訊");
            }

            var user = await _context.TMmemberLists.FirstOrDefaultAsync(u => u.MemberEmail == email);

            if (user == null)
            {
                byte[] profilePicData = await DownloadProfilePicture(profilePicUrl);
                user = new TMmemberList
                {
                    MemberName = name ?? "Google 用戶",
                    MemberEmail = email,
                    MemberPhoto = profilePicData,
                    Status = true
                };

                _context.TMmemberLists.Add(user);
                await _context.SaveChangesAsync();
            }

            var token = GenerateJwtToken(user);

            return Redirect($"http://localhost:4200/#/auth-success?token={token}");
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendVerificationCodeRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "電子郵件地址不能為空" });
            }
            if (_context.TMmemberLists.Any(u => u.MemberEmail == request.Email))
            {
                return BadRequest(new { status = false, message = "該 Email 已經註冊" });
            }

            // 檢查一小時內的驗證次數
            const int maxAttemptsPerHour = 5;
            var now = DateTime.UtcNow;
            var oneHourAgo = now.AddHours(-1);

            // 初始化該用戶的驗證記錄（如果不存在）
            if (!_verificationAttempts.ContainsKey(request.Email))
            {
                _verificationAttempts[request.Email] = new List<DateTime>();
            }

            // 移除超過一小時的記錄
            _verificationAttempts[request.Email].RemoveAll(t => t < oneHourAgo);

            // 檢查是否超過限制
            if (_verificationAttempts[request.Email].Count >= maxAttemptsPerHour)
            {
                return BadRequest(new { message = "一小時內驗證次數已達上限（5次），請稍後再試" });
            }

            // 生成6位隨機驗證碼
            var code = new Random().Next(100000, 999999).ToString();
            var expiryTime = DateTime.UtcNow.AddMinutes(5);
            _verificationCodes[request.Email] = (code, expiryTime);

            try
            {
                await _emailService.SendVerificationCodeAsync(request.Email, code);
                // 記錄本次驗證請求時間
                _verificationAttempts[request.Email].Add(now);
                return Ok(new { message = "驗證碼已發送" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"發送驗證碼失敗: {ex.Message}" });
            }
        }

        [HttpPost("register-with-verification")]
        public async Task<IActionResult> RegisterWithVerification([FromBody] RegisterWithVerificationRequest request)
        {
            if (!_verificationCodes.ContainsKey(request.Email))
            {
                return BadRequest(new { message = "未找到驗證碼，請先請求驗證碼" });
            }

            var (storedCode, expiryTime) = _verificationCodes[request.Email];

            if (DateTime.UtcNow > expiryTime)
            {
                _verificationCodes.Remove(request.Email);
                return BadRequest(new { message = "驗證碼已過期，請重新請求" });
            }

            if (storedCode != request.VerificationCode)
            {
                return BadRequest(new { message = "驗證碼無效" });
            }

            if (_context.TMmemberLists.Any(u => u.MemberEmail == request.Email))
            {
                return BadRequest(new { status = false, message = "該 Email 已經註冊" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new TMmemberList
            {
                MemberName = request.Name,
                MemberEmail = request.Email,
                MemberPassword = hashedPassword,
                RecentLogin = DateTime.UtcNow,
                Status = true
            };

            _context.TMmemberLists.Add(newUser);
            await _context.SaveChangesAsync();

            _verificationCodes.Remove(request.Email);
            // 可選：註冊成功後清除該用戶的驗證記錄
            _verificationAttempts.Remove(request.Email);

            var token = GenerateJwtToken(newUser);
            return Ok(new { status = true, message = "註冊成功", token });
        }

        private async Task<byte[]> DownloadProfilePicture(string profilePicUrl)
        {
            if (string.IsNullOrEmpty(profilePicUrl))
                return new byte[0];

            try
            {
                var imageData = await _httpClient.GetByteArrayAsync(profilePicUrl);
                Console.WriteLine($"ProfilePic 下載成功，大小: {imageData.Length} bytes");
                return imageData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下載 ProfilePic 失敗: {ex.Message}");
                return new byte[0];
            }
        }

        private string GenerateJwtToken(TMmemberList user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aPj4eQm9TzGdK7xF5sLzN3vW8HcJ1dXq"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            string profilePicBase64 = (user.MemberPhoto != null && user.MemberPhoto.Length > 0)
                ? Convert.ToBase64String(user.MemberPhoto)
                : "";
            var claims = new[]
            {
                new Claim("userId", user.MemberId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.MemberEmail),
                new Claim("name", user.MemberName),
                new Claim("profilePic", profilePicBase64)
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
    }

    // 請求模型
    public class SendVerificationCodeRequest
    {
        public string Email { get; set; }
    }

    public class RegisterWithVerificationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
    }
}