using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models.Entities
{
    public class BookSubCategory
    {
        public short? SubCategoriesId { get; set; }

        [ForeignKey(nameof(SubCategoriesId))]
        public SubCategory? SubCategory { get; set; }

        public long? BooksId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }
    }
}
