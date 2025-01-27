using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TCcourseFavorite
{
    public int CourseFavoriteId { get; set; }

    public int? CourseId { get; set; }

    public int? MemberId { get; set; }

    public virtual TCcourse? Course { get; set; }
}
