using diveWebAPI.Models;

namespace diveWebAPI.DTO
{
    public class TNorderDTO
    {
        public int OrderId { get; set; }

        public int? MemberId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? ShipAddress { get; set; }

        public string? ShipPhone { get; set; }

        public string? OrderStatus { get; set; }

        public decimal? TotalAmount { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? MerchantTradeNo { get; set; }

        public List<TNorderDetailDTO> OrderDetails { get; set; } = new();


    }
}

