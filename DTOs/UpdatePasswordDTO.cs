using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.DTOs
{
	public class UpdatePasswordDTO
	{
        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

