namespace diveWebAPI.DTO
{
    public class TMorderDTO
    {
        public string OrderType { get; set; }
        public int OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipPhone { get; set; }
        public string? PaymentMethod { get; set; }
        public int? OrderStatusId { get; set; }
        public int? OrderLogId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? SiteId { get; set; }
        public List<OrderDetailDTO> Details { get; set; } = new List<OrderDetailDTO>(); 
    }

    public class OrderDetailDTO
    {
        public string ItemName { get; set; } 
        public int Quantity { get; set; }    
        public decimal TotalPrice { get; set; } 
    }
}