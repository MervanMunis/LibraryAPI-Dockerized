using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities.Models
{
    public class BookRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BookRatingId { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0-5")]
        public float? GivenRating { get; set; } = 0;

        [ForeignKey(nameof(Book))]
        public long BookId { get; set; }

        public string MemberId { get; set; } = string.Empty;

        [JsonIgnore]
        public Book? Book { get; set; }
    }
}
