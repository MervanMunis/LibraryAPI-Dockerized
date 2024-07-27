using System;
namespace LibraryAPI.DTOs.Request
{
	public class AuthorRequest
	{
        public string FullName { get; set; } = string.Empty;

        public string? Biography { get; set; }

        public short? BirthYear { get; set; }

        public short? DeathYear { get; set; }

        public short LanguageId { get; set; }
    }
}

