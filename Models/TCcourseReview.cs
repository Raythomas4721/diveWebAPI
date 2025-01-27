using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TCcourseReview
{
    public int CourseReviewId { get; set; }

    public int? CourseId { get; set; }

    public int? OrderId { get; set; }

    public int? MemberId { get; set; }

    public int? Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateTime? ReviewDate { get; set; }

    public virtual TCcourse? Course { get; set; }

    public virtual TMmemberList? Member { get; set; }

    public virtual TCorder? Order { get; set; }
}
