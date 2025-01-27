using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TUproductCondition
{
    public int ProductConditionId { get; set; }

    public string? Condition { get; set; }

    public virtual ICollection<TUproduct> TUproducts { get; set; } = new List<TUproduct>();
}
