using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TCcourseCategory
{
    public int CourseCategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }

    public int? Duration { get; set; }

    public int? Quota { get; set; }

    public virtual ICollection<TCcourse> TCcourses { get; set; } = new List<TCcourse>();
}
