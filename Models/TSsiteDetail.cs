using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TSsiteDetail
{
    public int SiteId { get; set; }

    public string? VenueName { get; set; }

    public int? NumberOfPeople { get; set; }

    public string? VenueAddress { get; set; }

    public string Detail { get; set; } = null!;

    public byte[]? Photo { get; set; }

    public string? Evaluate { get; set; }

    public int? Collect { get; set; }

    public virtual ICollection<TSorder> TSorders { get; set; } = new List<TSorder>();

    public virtual ICollection<TSphoto> TSphotos { get; set; } = new List<TSphoto>();
}
