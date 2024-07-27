using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Request
{
    public class SubCategoryRequest
    {
        public string Name { get; set; } = string.Empty;

        public short? CategoryId { get; set; }
    }
}
