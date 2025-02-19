
namespace diveWebAPI.DTO
{
    public class TNproductDTO
    {
        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public decimal? UnitPrice { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public decimal? DiscountPrice { get; set; }  // 如果有折扣，計算後的價格
        public bool IsOnSale { get; set; }          // 是否在折扣中
        public DateTime? DiscountStart { get; set; }
        public DateTime? DiscountEnd { get; set; }
        public string? DiscountName { get; set; }
    

}
}
