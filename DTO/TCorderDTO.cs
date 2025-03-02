namespace diveWebAPI.DTO
{
    public class TCorderDTO
    {
        public int OrderId { get; set; }

        public int? MemberId { get; set; }
        public string? MemberName { get; set; } 


        public int? CourseId { get; set; }
        public string? CourseName { get; set; } = "未知課程";//Model裡沒有，顯示需要，自TCcourses & TCcourseCategory中的欄位拼接

        //課程名稱是字串拼接成的
        

        public decimal? CoursePrice { get; set; }

        public int? Quantity { get; set; }

        public DateTime? OrderDate { get; set; }
        public bool? OrderStatus { get; set; }
        public string CategoryName { get; set; } = "未分類";

        public string LevelName { get; set; } = "無等級";
        public string CoachName { get; set; } = "無教練";
        public DateTime? StartAt { get; set; }
        public string? Photo { get; set; }

    }
}
