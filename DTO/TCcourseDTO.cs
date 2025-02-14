namespace diveWebAPI.DTO
{
    public class TCcourseDTO
    {
        public int CourseId { get; set; }

        public int? CourseCategoryId { get; set; }

        public int? LevelId { get; set; }

        public int? CoachId { get; set; }

        public decimal? CoursePrice { get; set; }

        public byte[]? Photo { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Discription { get; set; }

        public bool? CourseStatus { get; set; }

        public DateTime? StartAt { get; set; }
    }
}
