namespace diveWebAPI.DTO
{
    public class TNorderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int? ProductvariantsId { get; set; }
        public decimal? UnitPriceAtOrder { get; set; }
        public int? Quantity { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? Subtotal { get; set; }
    }
}
