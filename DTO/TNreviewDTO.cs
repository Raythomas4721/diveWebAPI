using diveWebAPI.Models;

namespace diveWebAPI.DTO
{
    public class TNreviewDTO
    {

        public int ReviewId { get; set; }

        public int? MemberId { get; set; }

        public string ReviewContent { get; set; }

        public int? ReviewRating { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ProductId { get; set; }

    }
}
