using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TUorderLog
{
    public int OrderLogId { get; set; }

    public int? OrderId { get; set; }

    public int? OrderStatusId { get; set; }

    public DateTime? StatusDate { get; set; }

    public string? Remarks { get; set; }

    public virtual TUorder? Order { get; set; }

    public virtual TUorderStatusId? OrderStatus { get; set; }
}
