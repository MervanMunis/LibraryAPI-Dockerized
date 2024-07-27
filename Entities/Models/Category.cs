using LibraryAPI.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short CategoryId { get; set; }

        [Required]
        [StringLength(800)]
        [Column(TypeName = "varchar(800)")]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public string CategoryStatus { get; set; } = Status.Active.ToString();

        [JsonIgnore]
        public ICollection<SubCategory>? SubCategories { get; set; }
    }
}
