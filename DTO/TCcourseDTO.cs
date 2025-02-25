namespace diveWebAPI.DTO
{
    public class TCcourseDTO
    {
        public int CourseId { get; set; }

        //public int? CourseCategoryId { get; set; }

        public string? CategoryName { get; set; }

        //public int? LevelId { get; set; }
        public string? LevelName { get; set; }

        //public int? CoachId { get; set; }

        public string? CoachName { get; set; }

        public decimal? CoursePrice { get; set; }

        public byte[]? Photo { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Discription { get; set; }

        public bool? CourseStatus { get; set; }

        public DateTime? StartAt { get; set; }

        public string Description { get; set; }  // 新增
        public string Gender { get; set; } // 新增
        public string Experience { get; set; }
        public byte[] CoachPhoto { get; set; }
    }
}
