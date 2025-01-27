using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNgender
{
    public int GenderId { get; set; }

    public string? Gender { get; set; }

    public virtual ICollection<TNproductvariant> TNproductvariants { get; set; } = new List<TNproductvariant>();
}
