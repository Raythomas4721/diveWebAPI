namespace diveWebAPI.DTO
{
    public class TNorderItemDTO
    {
        public int ProductvariantsId { get; set; }
        public decimal UnitPriceAtOrder { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountAmount { get; set; } // 若沒有就可移除
    }
}
