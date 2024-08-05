using System.Text.Json.Serialization;

namespace LibraryAPI.Models.DTOs.Request
{
    public class BookRatingRequest
    {
        public float GivenRating { get; set; }

        [JsonIgnore]
        public string? MemberId { get; set; }
    }
}
