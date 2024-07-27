﻿using LibraryAPI.Entities.Enums;

namespace LibraryAPI.DTOs.Response
{
    public class LanguageResponse
	{
        public short LanguageId { get; set; }

        public string? Name { get; set; }

        public string? LanguageStatus { get; set; }

        public string? NationalityName { get; set; }
    }
}

