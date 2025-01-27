using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNsize
{
    public int SizeId { get; set; }

    public string? Size { get; set; }

    public virtual ICollection<TNproductvariant> TNproductvariants { get; set; } = new List<TNproductvariant>();
}
