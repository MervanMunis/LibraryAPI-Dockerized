using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Response
{
    public class SubCategoryResponse
    {
        public short SubCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? SubCategoryStatus { get; set; } 
    }
}
