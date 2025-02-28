namespace diveWebAPI.Models.SiteDTO
{
    public class TSsiteDetailUpdateDTO
    {
        public int SiteId { get; set; } // 包含 SiteId 以確保我們更新正確的實體
        public string? VenueName { get; set; }
        public int? NumberOfPeople { get; set; }
        public string? VenueAddress { get; set; }
        public string? Detail { get; set; }
        public string? Evaluate { get; set; }
        public int? Collect { get; set; }
        public decimal? SitePrice { get; set; }

        public string? SiteSize { get; set; }

        public string? SitePhone { get; set; }

        public string? SiteEmail { get; set; }
        public int? region { get; set; }
    }

    public class TSsiteDetailUpdateWithPhotoDTO
    {
        public int SiteId { get; set; } // 包含 SiteId 以確保我們更新正確的實體
        public string? VenueName { get; set; }
        public int? NumberOfPeople { get; set; }
        public string? VenueAddress { get; set; }
        public string? Detail { get; set; }
        public IFormFile? Photo { get; set; } // 用於上傳檔案
        public string? Evaluate { get; set; }
        public int? Collect { get; set; }
        public decimal? SitePrice { get; set; }

        public string? SiteSize { get; set; }

        public string? SitePhone { get; set; }

        public string? SiteEmail { get; set; }
    }
}
