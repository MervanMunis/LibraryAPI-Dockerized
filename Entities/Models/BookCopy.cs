using LibraryAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Entities.Models
{
    public class BookCopy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BookCopyId { get; set; }                               // BUNA UNİQE KEY VERMEYİ DÜŞÜN YA DA BOOKID ÜZERİNDEN DİĞER BOOKCOPYIDLERİ TANIMLA 

        public string BookCopyStatus { get; set; } = Status.Active.ToString();

        [ForeignKey(nameof(Book))]
        public long BookId { get; set; }

        [JsonIgnore]
        public Book? Book { get; set; }

        [JsonIgnore]
        public ICollection<Loan>? Loans { get; set; }
    }
}
