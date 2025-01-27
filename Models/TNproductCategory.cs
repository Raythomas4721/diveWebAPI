using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNproductCategory
{
    public int ProductCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? ParentCategoryId { get; set; }
}
