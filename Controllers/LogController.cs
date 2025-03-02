using diveWebAPI.DTO;
using diveWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace diveWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly DiveShopperContext _context;

        public LogController(DiveShopperContext context)
        {
            _context = context;
        }
        [HttpPost("behavior")]
        public async Task<BehaviorLogDTO> PostBehaviorLog([FromBody] BehaviorLogDTO dto)
        {
            // BehaviorLogDto 可包含 guestId, memberId, productId, eventType, eventTime, ...
            var entity = new UserBehaviorLog
            {
                GuestId = dto.GuestId,
                MemberId = dto.MemberId,
                ProductId = dto.ProductId,
                EventType = dto.EventType,
                EventTime = dto.EventTime,
                
                IpAddress = dto.IpAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString(),
               
                DwellTime = dto.DwellTime,
                ExtraData = dto.ExtraData
            };

            _context.UserBehaviorLogs.Add(entity);
            await _context.SaveChangesAsync();
            Console.WriteLine(dto.MemberId);
            return dto;

        }
    }
}

