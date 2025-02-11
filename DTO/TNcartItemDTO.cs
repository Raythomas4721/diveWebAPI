
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace diveWebAPI.DTO
{
    public class TNcartItemDTO
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartitemId { get; set; }

        public int? MemberId { get; set; }

        public int? UproductId { get; set; }

        public int? ProductvariantsId { get; set; }

        public string ProductName { get; set; }

        public int? Quantity { get; set; }

        public decimal? UnitpriceatCart { get; set; }

        public bool? IsLocked { get; set; }

        public string? Condition { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        
    }
}
