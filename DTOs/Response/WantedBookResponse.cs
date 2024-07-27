using LibraryAPI.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Response
{
    public class WantedBookResponse
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Languages { get; set; }

        public List<string>? SubCategories { get; set; }
    }
}
