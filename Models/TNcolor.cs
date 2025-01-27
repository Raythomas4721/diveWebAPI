using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNcolor
{
    public int ColorId { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<TNproductvariant> TNproductvariants { get; set; } = new List<TNproductvariant>();
}
