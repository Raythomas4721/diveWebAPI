using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TMcoach
{
    public int CoachId { get; set; }

    public string? CoachName { get; set; }

    public string? Gender { get; set; }

    public string? Experience { get; set; }

    public int? DivingStyleId { get; set; }

    public byte[]? CoachPhoto { get; set; }

    public string? CoachPhone { get; set; }

    public virtual ICollection<TCcourse> TCcourses { get; set; } = new List<TCcourse>();

    public virtual ICollection<TMcoachDivingLevel> TMcoachDivingLevels { get; set; } = new List<TMcoachDivingLevel>();

    public virtual ICollection<TMcoachDiving> TMcoachDivings { get; set; } = new List<TMcoachDiving>();
}
