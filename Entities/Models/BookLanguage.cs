using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities.Models
{
    public class BookLanguage
    {
        [ForeignKey(nameof(Language))]
        public short? LanguagesId { get; set; }
        public Language? Language { get; set; }


        [ForeignKey(nameof(Book))]
        public long? BooksId { get; set; }
        [JsonIgnore]
        public Book? Book { get; set; }
    }
}
