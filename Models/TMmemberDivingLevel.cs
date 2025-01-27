using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TMmemberDivingLevel
{
    public int DivingLevelId { get; set; }

    public int? LevelId { get; set; }

    public int? MemberId { get; set; }

    public virtual TMdivingLevel? Level { get; set; }

    public virtual TMmemberList? Member { get; set; }
}
