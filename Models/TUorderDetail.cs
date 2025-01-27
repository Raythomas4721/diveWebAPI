using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TUorderDetail
{
    public int OrderDetailsId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public virtual TUorder? Order { get; set; }

    public virtual TUproduct? Product { get; set; }

    public virtual ICollection<TUreview> TUreviews { get; set; } = new List<TUreview>();
}
