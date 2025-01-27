using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TUcategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<TUproduct> TUproducts { get; set; } = new List<TUproduct>();
}
