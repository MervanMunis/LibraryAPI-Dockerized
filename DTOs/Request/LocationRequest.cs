using LibraryAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs.Request
{
    public class LocationRequest
    {
        public string SectionCode { get; set; } = string.Empty;

        public string AisleCode { get; set; } = string.Empty;

        public string ShelfNumber { get; set; } = string.Empty;
    }
}
