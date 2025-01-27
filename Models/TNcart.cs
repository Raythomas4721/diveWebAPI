using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNcart
{
    public int CartId { get; set; }

    public int? MemberId { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual TMmemberList? Member { get; set; }

    public virtual ICollection<TNcartItem> TNcartItems { get; set; } = new List<TNcartItem>();
}
