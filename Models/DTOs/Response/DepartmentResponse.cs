using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.DTOs.Response
{
    public class DepartmentResponse
    {
        public short DepartmentId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
