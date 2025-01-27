using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TMdivingLevelName
{
    public int LevelNameId { get; set; }

    public string? LevelTypeName { get; set; }

    public virtual ICollection<TMdivingLevel> TMdivingLevels { get; set; } = new List<TMdivingLevel>();
}
