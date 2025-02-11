namespace diveWebAPI.DTO
{
    public class TopProductDTO
    {
       

        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        public string ImageUrl { get; set; }

        public decimal? UnitPrice { get; set; }

        public int? ViewCount { get; set; }

        //public int? CartCount { get; set; }

        //public int? PurchaseCount { get; set; }
    }
}
