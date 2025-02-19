
namespace diveWebAPI.DTO
{
    public class BehaviorLogDTO
    {
        

        public string GuestId { get; set; }

        public int? MemberId { get; set; }

        public int? ProductId { get; set; }

        public string? EventType { get; set; }

        public DateTime? EventTime { get; set; }

        public string? IpAddress { get; set; }

        public int? DwellTime { get; set; }

        public string? ExtraData { get; set; }

        public DateTime? CreationDate { get; set; }

    }
}
