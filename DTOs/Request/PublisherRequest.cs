using LibraryAPI.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace LibraryAPI.DTOs.Request
{
	public class PublisherRequest
	{
        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? ContactPerson { get; set; }
    }
}

