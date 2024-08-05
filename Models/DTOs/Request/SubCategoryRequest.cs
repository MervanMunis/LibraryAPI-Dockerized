namespace LibraryAPI.Models.DTOs.Request
{
    public class SubCategoryRequest
    {
        public string Name { get; set; } = string.Empty;

        public short? CategoryId { get; set; }
    }
}
