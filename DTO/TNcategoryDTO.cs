namespace diveWebAPI.DTO
{
    public class TNcategoryDTO
    {
        public int ProductCategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? ParentCategoryId { get; set; }

        public string? imageFileName { get; set; }
    }
}
