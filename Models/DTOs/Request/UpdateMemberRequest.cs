using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.DTOs.Request
{
    public class UpdateMemberRequest
    {
        public string IdNumber { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public DateTime BirthDate { get; set; }

        public string? EducationalDegree { get; set; }
    }
}
