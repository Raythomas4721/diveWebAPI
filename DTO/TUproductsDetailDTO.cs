using Microsoft.AspNetCore.Mvc;

namespace diveWebAPI.DTO
{
    public class TUproductsDetailDTO 
    {
        public int ProductId { get; set; }

        public int? SellerId { get; set; }

        public int? CategoryId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public decimal? ProductPrice { get; set; }


        public DateTime? UpdatedAt { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? ProductConditionId { get; set; }

        public bool? ProductStatus { get; set; }

        //public string[] TUproductImages { get; set; }
        //public string[] TUproductImages { get; set; } = Array.Empty<string>();
        // 確保圖片列表為空陣列而非 null
        private string[] _tUproductImages = Array.Empty<string>();
        public string[] TUproductImages
        {
            get => _tUproductImages;
            set => _tUproductImages = value ?? Array.Empty<string>(); // 防止 null
        }
    }
}
