using LibraryAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models.Entities
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

        public short? CategoryId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        [JsonIgnore]
        public ICollection<BookSubCategory>? BookSubCategories { get; set; }
    }
}
