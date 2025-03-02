using diveWebAPI.Models;

namespace diveWebAPI.DTO
{
    public class TNdiscountDTO

    {
            public int DiscountId { get; set; }

            public string DiscountName { get; set; }

            public int? ProductCategoryId { get; set; }

            public decimal? DiscountValue { get; set; }

            public DateTime? StartDate { get; set; }

            public DateTime? EndDate { get; set; }

            public virtual TNproductCategory ProductCategory { get; set; }
        }
    }

