using System;
using LibraryAPI.Entities.Enums;

namespace LibraryAPI.DTOs.Request
{
	public class LanguageRequest
	{
        public string Name { get; set; } = string.Empty;

        public short NationalityId { get; set; }
    }
}

