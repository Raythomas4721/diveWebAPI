using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TNorder
{
    public int OrderId { get; set; }

    public int? MemberId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ShipAddress { get; set; }

    public string? ShipPhone { get; set; }

    public string? OrderStatus { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual TMmemberList? Member { get; set; }

    public virtual ICollection<TNorderDetail> TNorderDetails { get; set; } = new List<TNorderDetail>();
}
