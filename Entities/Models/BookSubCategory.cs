using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities.Models
{
    public class BookSubCategory
    {
        [ForeignKey(nameof(SubCategory))]

        public short? SubCategoriesId { get; set; }
        public SubCategory? SubCategory { get; set; }

        [ForeignKey(nameof(Book))]
        public long? BooksId { get; set; }

        [JsonIgnore]
        public Book? Book { get; set; }
    }
}
