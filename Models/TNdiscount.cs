using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNdiscount
{
    public int DiscountId { get; set; }

    public string? DiscountName { get; set; }

    public int? OrderDetailId { get; set; }

    public decimal? DiscountValue { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<TNorderDetail> TNorderDetails { get; set; } = new List<TNorderDetail>();
}
