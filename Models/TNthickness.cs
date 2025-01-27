using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNthickness
{
    public int ThicknessId { get; set; }

    public string? Thickness { get; set; }

    public virtual ICollection<TNproductvariant> TNproductvariants { get; set; } = new List<TNproductvariant>();
}
