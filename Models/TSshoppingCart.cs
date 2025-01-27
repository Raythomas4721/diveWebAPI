using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TSshoppingCart
{
    public int CartId { get; set; }

    public int? MemberId { get; set; }

    public DateOnly? Date { get; set; }

    public string? ScheduleId { get; set; }

    public virtual TMmemberList? Member { get; set; }
}
