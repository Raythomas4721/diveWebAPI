using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using diveWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly diveShopperContext _context;
        private readonly HttpClient _httpClient;

        public AccountController(diveShopperContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
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

            // 獲取 Google 用戶資訊
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
        private async Task<byte[]> DownloadProfilePicture(string profilePicUrl)
        {
            if (string.IsNullOrEmpty(profilePicUrl))
                return new byte[0]; // 若沒有大頭貼則回傳空陣列

            try
            {
                var imageData = await _httpClient.GetByteArrayAsync(profilePicUrl);
                Console.WriteLine($"ProfilePic 下載成功，大小: {imageData.Length} bytes"); 
                return imageData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下載 ProfilePic 失敗: {ex.Message}");
                return new byte[0]; // 若下載失敗，回傳空
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
}
