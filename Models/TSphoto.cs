using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TSphoto
{
    public int PhotoId { get; set; }

    public int? SiteId { get; set; }

    public byte[]? Photo1 { get; set; }

    public byte[]? Photo2 { get; set; }

    public byte[]? Photo3 { get; set; }

    public virtual TSsiteDetail? Site { get; set; }
}
