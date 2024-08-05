using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Models.DTOs.Request
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
