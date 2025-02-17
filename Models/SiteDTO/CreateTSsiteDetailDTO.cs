namespace diveWebAPI.Models.SiteDTO
{
    public class CreateTSsiteDetailDTO
    {
        public string? VenueName { get; set; }
        public int? NumberOfPeople { get; set; }
        public string? VenueAddress { get; set; }
        public string Detail { get; set; } = null!;
        public IFormFile? Photo { get; set; }
        public string? Evaluate { get; set; }
        public int? Collect { get; set; }
    }
}
