namespace diveWebAPI.DTO
{
    public class TNcreateOrderDTO
    {
        public int MemberId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipPhone { get; set; }
        // 其他需要的欄位 ex: CouponCode, ShippingMethod, ...

        public List<TNorderItemDTO> OrderItems { get; set; } = new List<TNorderItemDTO>();
    }
}
