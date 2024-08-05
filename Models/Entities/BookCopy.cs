using LibraryAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models.Entities
{
    public class BookCopy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long BookCopyId { get; set; }                               // BUNA UNİQE KEY VERMEYİ DÜŞÜN YA DA BOOKID ÜZERİNDEN DİĞER BOOKCOPYIDLERİ TANIMLA 

        public string BookCopyStatus { get; set; } = Status.Active.ToString();

        public long BookId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [JsonIgnore]
        public ICollection<Loan>? Loans { get; set; }
    }
}
