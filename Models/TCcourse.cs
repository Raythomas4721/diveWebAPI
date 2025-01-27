using System;
using System.Collections.Generic;

namespace diveWebAPI.Models;

public partial class TCcourse
{
    public int CourseId { get; set; }

    public int? CourseCategoryId { get; set; }

    public int? LevelId { get; set; }

    public int? CoachId { get; set; }

    public decimal? CoursePrice { get; set; }

    public byte[]? Photo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Discription { get; set; }

    public bool? CourseStatus { get; set; }

    public virtual TMcoach? Coach { get; set; }

    public virtual TCcourseCategory? CourseCategory { get; set; }

    public virtual TCcourseLevel? Level { get; set; }

    public virtual ICollection<TCcourseFavorite> TCcourseFavorites { get; set; } = new List<TCcourseFavorite>();

    public virtual ICollection<TCcourseReview> TCcourseReviews { get; set; } = new List<TCcourseReview>();

    public virtual ICollection<TCorderDetail> TCorderDetails { get; set; } = new List<TCorderDetail>();
}
