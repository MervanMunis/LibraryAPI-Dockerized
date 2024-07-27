using LibraryAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Request
{
    public class CategoryRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
