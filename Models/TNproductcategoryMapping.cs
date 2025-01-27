using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNproductcategoryMapping
{
    public int? ProductId { get; set; }

    public int? ProductCategoryId { get; set; }

    public virtual TNproduct? Product { get; set; }

    public virtual TNproductCategory? ProductCategory { get; set; }
}
