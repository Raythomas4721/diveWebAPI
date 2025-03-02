using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using diveWebAPI.Models;
using diveWebAPI.Models.SiteDTO;
using Microsoft.AspNetCore.Hosting; // 引入 IWebHostEnvironment
using System.IO;
using System.Text.Json;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TSsiteDetailsController : ControllerBase
    {
        private readonly DiveShopperContext _context;
        private readonly IWebHostEnvironment _environment; // 注入 IWebHostEnvironment
        private readonly IHttpClientFactory _clientFactory; // 注入 IHttpClientFactory
        private readonly string CWA_API_KEY = "CWA-1335697B-71F0-4EF6-94C4-9CF794DE7055"; // 你的氣象署授權碼

        public TSsiteDetailsController(DiveShopperContext context, IWebHostEnvironment environment, IHttpClientFactory clientFactory)
        {
            _context = context;
            _environment = environment;
            _clientFactory = clientFactory;
        }
        //讀取
        // GET: api/TSsiteDetails
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TSsiteDetail>>> GetTSsiteDetails()
        //{
        //    return await _context.TSsiteDetails.ToListAsync();
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TSsiteDetail>>> GetTSsiteDetails(int region)
        {
            IQueryable<TSsiteDetail> query = _context.TSsiteDetails;



            // 根據區域篩選場地，這裡假設 VenueAddress 包含區域資訊
            // 你可能需要調整篩選條件，以符合你的實際資料結構
            if (region == 1)
            {
                query = query.Where(s => s.Region == 1);
            }
            else if (region == 2)
            {
                query = query.Where(s => s.Region == 2);
            }
            else if (region == 3)
            {
                query = query.Where(s => s.Region == 3);
            }
            else if (region == 4)
            {
                query = query.Where(s => s.Region == 4);
            }
            else 
            {
                query.ToList();
                //return await query.ToListAsync();
            }
            return query.ToList();
        }




        // GET: api/TSsiteDetails/Search?keyword=yourKeyword
        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<TSsiteDetail>>> SearchTSsiteDetails(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return await GetTSsiteDetails(0); // 如果沒有關鍵字，則回傳所有場地
            }

            //使用關鍵字搜尋 VenueName 或 Detail 欄位
           var results = await _context.TSsiteDetails
                .Where(s => s.VenueName.Contains(keyword) || s.Detail.Contains(keyword))
                .ToListAsync();

            if (results == null || results.Count == 0)
            {
                return NotFound("找不到符合條件的場地。");
            }

            return results;
        }

        // GET: api/TSsiteDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TSsiteDetail>> GetTSsiteDetail(int id)
        {
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);

            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            return tSsiteDetail;
        }
        //修改
        // PUT: api/TSsiteDetails/5 (更新不含照片)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTSsiteDetail(int id, TSsiteDetailUpdateDTO tSsiteDetailDto)
        {
            if (id != tSsiteDetailDto.SiteId)
            {
                return BadRequest();
            }

            // 1. 先確認 SiteId 對應的資料是否存在
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);
            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            // 2. 將 DTO 的值更新到現有的實體
            tSsiteDetail.VenueName = tSsiteDetailDto.VenueName;
            tSsiteDetail.NumberOfPeople = tSsiteDetailDto.NumberOfPeople;
            tSsiteDetail.VenueAddress = tSsiteDetailDto.VenueAddress;
            tSsiteDetail.Detail = tSsiteDetailDto.Detail;
            tSsiteDetail.Evaluate = tSsiteDetailDto.Evaluate;
            tSsiteDetail.SitePrice = tSsiteDetailDto.SitePrice;
            tSsiteDetail.SiteSize = tSsiteDetailDto.SiteSize;
            tSsiteDetail.SitePhone = tSsiteDetailDto.SitePhone;
            tSsiteDetail.SiteEmail = tSsiteDetailDto.SiteEmail;
            tSsiteDetail.Region = tSsiteDetailDto.region;


            _context.Entry(tSsiteDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSsiteDetailExists(id))
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

        // PUT: api/TSsiteDetails/5/WithPhoto (更新含照片)
        [HttpPut("{id}/WithPhoto")]
        public async Task<IActionResult> PutTSsiteDetailWithPhoto(int id, [FromForm] TSsiteDetailUpdateWithPhotoDTO tSsiteDetailDto)
        {
            if (id != tSsiteDetailDto.SiteId)
            {
                return BadRequest();
            }

            // 1. 先確認 SiteId 對應的資料是否存在
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);
            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            // 2. 將 DTO 的值更新到現有的實體
            tSsiteDetail.VenueName = tSsiteDetailDto.VenueName;
            tSsiteDetail.NumberOfPeople = tSsiteDetailDto.NumberOfPeople;
            tSsiteDetail.VenueAddress = tSsiteDetailDto.VenueAddress;
            tSsiteDetail.Detail = tSsiteDetailDto.Detail;
            tSsiteDetail.Evaluate = tSsiteDetailDto.Evaluate;
            tSsiteDetail.Collect = tSsiteDetailDto.Collect;
            tSsiteDetail.SitePrice = tSsiteDetailDto.SitePrice;
            tSsiteDetail.SiteSize = tSsiteDetailDto.SiteSize;
            tSsiteDetail.SitePhone = tSsiteDetailDto.SitePhone;
            tSsiteDetail.SiteEmail = tSsiteDetailDto.SiteEmail;
            tSsiteDetail.Region = tSsiteDetailDto.region;


            // 3. 處理照片上傳
            if (tSsiteDetailDto.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await tSsiteDetailDto.Photo.CopyToAsync(memoryStream);
                    tSsiteDetail.Photo = memoryStream.ToArray(); // 將圖片轉換為 byte[]
                }
            }

            _context.Entry(tSsiteDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TSsiteDetailExists(id))
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

        // (Optional) 儲存照片的範例方法
        private async Task<string> SavePhoto(IFormFile photo)
        {
            // 產生一個獨一無二的檔案名稱
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            // 儲存路徑 (請根據您的專案設定調整)
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return "/uploads/" + fileName; // 回傳相對路徑
        }
        //新增
        // POST: api/TSsiteDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostTSsiteDetail")]
        public async Task<ActionResult<TSsiteDetail>> PostTSsiteDetail(TSsiteDetail tSsiteDetail)
        {
            _context.TSsiteDetails.Add(tSsiteDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSsiteDetail", new { id = tSsiteDetail.SiteId }, tSsiteDetail);
        }

        [HttpPost("PostTSsiteDetailPhoto")]
        public async Task<IActionResult> PostTSsiteDetailPhoto4564([FromForm] CreateTSsiteDetailDTO dto)
        {
            var tSsiteDetail = new TSsiteDetail
            {
                VenueName = dto.VenueName,
                NumberOfPeople = dto.NumberOfPeople,
                VenueAddress = dto.VenueAddress,
                Detail = dto.Detail,
                Evaluate = dto.Evaluate,
                Collect = dto.Collect,
                SitePrice = dto.SitePrice,
                SiteSize = dto.SiteSize,
                SitePhone = dto.SitePhone,
                SiteEmail = dto.SiteEmail,
                Region = dto.region,
            };

            // 處理上傳的圖片
            if (dto.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.Photo.CopyToAsync(memoryStream);
                    tSsiteDetail.Photo = memoryStream.ToArray(); // 將圖片轉換為 byte[]
                }
            }

            // 將 tSsiteDetail 物件添加到資料庫
            _context.TSsiteDetails.Add(tSsiteDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTSsiteDetail", new { id = tSsiteDetail.SiteId }, tSsiteDetail);
        }
        //刪除
        // DELETE: api/TSsiteDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTSsiteDetail(int id)
        {
            var tSsiteDetail = await _context.TSsiteDetails.FindAsync(id);
            if (tSsiteDetail == null)
            {
                return NotFound();
            }

            _context.TSsiteDetails.Remove(tSsiteDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TSsiteDetailExists(int id)
        {
            return _context.TSsiteDetails.Any(e => e.SiteId == id);
        }

        //[HttpGet("Area")]
        //public async Task<ActionResult<IEnumerable<TSsiteDetail>>> GetTSsiteDetailsarea(int area)
        //{
        //    IQueryable<TSsiteDetail> query = _context.TSsiteDetails;

        //    switch (area)
        //    {
        //        case 1:
        //            query = query.Where(s => s.area == 1); // 台北
        //            break;
        //        case 2:
        //            query = query.Where(s => s.area == 2); // 新北
        //            break;
        //        default:
        //            // 如果 region 值不是 1 或 2，則回傳所有資料
        //            break;
        //    }

        //    return await query.ToListAsync();
        //}
        [HttpGet("Weather")]
        public async Task<ActionResult<WeatherInfo>> GetWeather(string city)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City parameter is required.");
            }

            try
            {
                // 呼叫中央氣象局 API
                var weatherInfo = await GetWeatherFromCWA(city);
                if (weatherInfo == null)
                {
                    return NotFound("Weather information not found for the specified city.");
                }
                return Ok(weatherInfo);
            }
            catch (Exception ex)
            {
                // 記錄錯誤訊息
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
                return StatusCode(500, "Failed to retrieve weather information.");
            }
        }
        private async Task<WeatherInfo> GetWeatherFromCWA(string city)
        {
            // 氣象局API網址 (假設你想獲取的是"一般天氣預報-今明 36 小時天氣預報")
            string apiUrl = $"https://opendata.cwa.gov.tw/api/v1/rest/datastore/F-C0032-001?Authorization={CWA_API_KEY}&format=JSON&locationName={city}";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var client = _clientFactory.CreateClient(); // 使用 IHttpClientFactory 建立 HttpClient

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    // 使用 System.Text.Json 來解析 JSON
                    var document = await JsonDocument.ParseAsync(responseStream);
                    var locations = document.RootElement.GetProperty("records").GetProperty("location");

                    // 找到對應城市的天氣資料
                    foreach (var location in locations.EnumerateArray())
                    {
                        if (location.GetProperty("locationName").GetString() == city)
                        {
                            var weatherElement = location.GetProperty("weatherElement");
                            var minTemp = weatherElement[2].GetProperty("time")[0].GetProperty("parameter").GetProperty("parameterName").GetString();
                            var maxTemp = weatherElement[4].GetProperty("time")[0].GetProperty("parameter").GetProperty("parameterName").GetString();
                            var condition = weatherElement[0].GetProperty("time")[0].GetProperty("parameter").GetProperty("parameterName").GetString();

                            // 建立 WeatherInfo 物件
                            var weatherInfo = new WeatherInfo
                            {
                                City = city,
                                Temperature = $"{minTemp}°C - {maxTemp}°C",
                                Condition = condition
                            };
                            return weatherInfo;
                        }
                    }
                }
            }
            else
            {
                // 記錄錯誤訊息
                Console.WriteLine($"CWA API request failed with status code: {response.StatusCode}");
                return null;
            }
            return null;
        }
    }
    // 定義一個簡單的天氣資訊類別
    public class WeatherInfo
    {
        public string City { get; set; }
        public string Temperature { get; set; }
        public string Condition { get; set; }
    }
}
    

