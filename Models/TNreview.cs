using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNreview
{
    public int ReviewId { get; set; }

    public int? MemberId { get; set; }

    public int? OrderDetailsId { get; set; }

    public string? ReviewContent { get; set; }

    public int? ReviewRating { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TMmemberList? Member { get; set; }

    public virtual TNorderDetail? OrderDetails { get; set; }
}
