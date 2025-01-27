using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TMdivingLevel
{
    public int LevelId { get; set; }

    public string? LevelType { get; set; }

    public int? LevelNameId { get; set; }

    public virtual TMdivingLevelName? LevelName { get; set; }

    public virtual ICollection<TMcoachDivingLevel> TMcoachDivingLevels { get; set; } = new List<TMcoachDivingLevel>();

    public virtual ICollection<TMmemberDivingLevel> TMmemberDivingLevels { get; set; } = new List<TMmemberDivingLevel>();
}
