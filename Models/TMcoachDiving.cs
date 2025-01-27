using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TMcoachDiving
{
    public int CoachDivingId { get; set; }

    public int? CoachId { get; set; }

    public int? DivingStyleId { get; set; }

    public virtual TMcoach? Coach { get; set; }

    public virtual TMdivingStyle? DivingStyle { get; set; }
}
