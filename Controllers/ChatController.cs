using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using diveWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly DiveShopperContext _context;
        private readonly string _azureEndpoint;
        private readonly string _apiKey;
        // 用於儲存對話歷史
        private static readonly Dictionary<string, List<object>> ConversationHistory = new Dictionary<string, List<object>>();

        public ChatController(HttpClient httpClient, DiveShopperContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _azureEndpoint = configuration["AzureAI:Endpoint"];
            _apiKey = configuration["AzureAI:ApiKey"];
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("API 正常運行");
        }

        [HttpGet("test-db")]
        public async Task<IActionResult> TestDatabase()
        {
            var products = await _context.TNproducts.ToListAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> GetChatResponse([FromBody] ChatRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
                return BadRequest("請提供訊息");

            if (!_context.TNproducts.Any())
                return BadRequest("資料庫中沒有產品資料");

            // 臨時使用固定 sessionId 測試上下文
            string sessionId = "test-session"; // 固定值，模擬同一會話
            if (!ConversationHistory.ContainsKey(sessionId))
                ConversationHistory[sessionId] = new List<object>();

            // 抓產品資料
            var products = await _context.TNproducts
                .Select(p => new
                {
                    p.ProductName,
                    p.UnitPrice,
                    Variants = p.TNproductvariants.Select(v => new
                    {
                        Size = v.SizeId != null ? _context.TNsizes.FirstOrDefault(s => s.SizeId == v.SizeId).Size ?? "未知" : "未知",
                        RawColor = v.ColorId != null ? _context.TNcolors.FirstOrDefault(c => c.ColorId == v.ColorId).Color ?? "未知" : "未知",
                        Thickness = v.ThicknessId != null ? _context.TNthicknesses.FirstOrDefault(t => t.ThicknessId == v.ThicknessId).Thickness ?? "未知" : "未知",
                        Gender = v.GenderId != null ? _context.TNgenders.FirstOrDefault(g => g.GenderId == v.GenderId).Gender ?? "未知" : "未知",
                        Stock = v.Stock > 0 ? "有貨" : "無貨"
                    })
                })
                .ToListAsync();

            var productList = string.Join("\n", products.Select(p =>
                $"{p.ProductName}: 價格 ${p.UnitPrice}, 運送費 ${(p.UnitPrice > 1000 ? 200 : 65)}, " +
                $"尺寸: {string.Join(", ", p.Variants.Select(v => v.Size))}, " +
                $"顏色: {string.Join(", ", p.Variants.Select(v => GetColorName(v.RawColor)))}, " +
                $"性別: {string.Join(", ", p.Variants.Select(v => v.Gender))}, " +
                $"厚度: {string.Join(", ", p.Variants.Select(v => v.Thickness))}, " +
                $"庫存: {string.Join(", ", p.Variants.Select(v => v.Stock))}"));

            // 抓課程資料
            var allCourses = await _context.TCcourses
                .Where(c => c.CourseStatus == true)
                .Select(c => new
                {
                    CategoryName = c.CourseCategory.CategoryName,
                    c.CoursePrice,
                    LevelName = c.Level.LevelName,
                    CoachName = c.Coach.CoachName,
                    c.Discription, // 若應為 Description 請修正
                    c.StartAt,
                    Quota = c.CourseCategory.Quota
                })
                .ToListAsync();

            var courses = allCourses
                .GroupBy(c => c.CategoryName)
                .Select(g => g.OrderBy(c => c.StartAt).First())
                .ToList();

            var courseList = string.Join("\n", courses.Select(c =>
                $"{c.CategoryName}: 價格 ${c.CoursePrice}, 難易度: {c.LevelName}, " +
                $"教練: {c.CoachName}, 描述: {c.Discription}, 開始時間: {c.StartAt:yyyy-MM-dd}, " +
                $"總名額: {c.Quota}"));

            var systemPrompt = $"你是一個潛水裝備與課程客服機器人，根據以下資料和對話歷史簡潔回答客戶關於價格、運送費、尺寸、顏色、性別、庫存、課程價格、總名額、課程難易度和開始日期的問題，並根據要求推薦適合的潛水裝備或課程（浮潛、自由潛水、水肺潛水）：\n" +
                              $"產品資料：\n{productList}\n" +
                              $"課程資料（每類型挑選一個代表課程）：\n{courseList}\n" +
                              "規則：推薦課程時，優先選價格低或開始時間近的，適合自由潛水選相關課程或產品，回答簡短直接，避免冗長描述。\n" +
                              "如果用戶說「想了解更多」，根據前文判斷：若提到「課程」「課」「潛水課程」，回答「請前往我們的課程詳情頁了解更多：http://localhost:4200/#/courses」；若提到「商品」「裝備」「潛水裝備」，回答「請前往我們的商品頁了解更多：http://localhost:4200/#/shop」；若無法判斷，說「請問您想了解商品還是課程的更多資訊？」。\n" +
                              "如果不知道答案，說「抱歉，我目前不知道」。";

            // 構建對話歷史
            var messages = new List<object> { new { role = "system", content = systemPrompt } };
            messages.AddRange(ConversationHistory[sessionId]);
            messages.Add(new { role = "user", content = request.Message });

            var payload = new
            {
                messages = messages.ToArray(),
                max_tokens = 200
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", _apiKey);

            var response = await _httpClient.PostAsync(_azureEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
                var reply = jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                // 更新對話歷史
                ConversationHistory[sessionId].Add(new { role = "user", content = request.Message });
                ConversationHistory[sessionId].Add(new { role = "assistant", content = reply });
                if (ConversationHistory[sessionId].Count > 10)
                    ConversationHistory[sessionId] = ConversationHistory[sessionId].Skip(ConversationHistory[sessionId].Count - 10).ToList();

                return Ok(new { Reply = reply });
            }

            return StatusCode((int)response.StatusCode, $"呼叫 Azure AI 失敗: {responseString}");
        }

        private static string GetColorName(string rgb)
        {
            return rgb switch
            {
                "rgb(56, 124, 220)" => "藍色",
                "rgb(0, 0, 255)" => "藍色",
                "rgb(0, 0, 0)" => "黑色",
                "rgb(255, 255, 255)" => "白色",
                "rgb(246, 0, 123)" => "粉紅色",
                "rgb(234, 234, 234)" => "淺灰色",
                "無選擇" => "無選擇",
                _ => "未知"
            };
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}