namespace LibraryAPI.Models.DTOs.Request
{
    public class LanguageRequest
    {
        public string Name { get; set; } = string.Empty;

        public short NationalityId { get; set; }
    }
}

