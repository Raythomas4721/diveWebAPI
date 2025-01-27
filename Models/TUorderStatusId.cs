using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TUorderStatusId
{
    public int OrderStatusId { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual ICollection<TUorderLog> TUorderLogs { get; set; } = new List<TUorderLog>();
}
