using LibraryAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities.Models
{
    public class SubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short SubCategoryId { get; set; }

        [Required]
        [StringLength(800)]
        [Column(TypeName = "varchar(800)")]
        public string Name { get; set; } = string.Empty;

        public string SubCategoryStatus { get; set; } = Status.Active.ToString();

        [ForeignKey(nameof(Category))]
        public short? CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }


        [JsonIgnore]
        public ICollection<BookSubCategory>? BookSubCategories { get; set; }
    }
}
