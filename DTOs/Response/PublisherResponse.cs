using System;
using LibraryAPI.Entities.Enums;
using LibraryAPI.Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.DTOs.Response
{
	public class PublisherResponse
	{
        public long PublisherId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? ContactPerson { get; set; }

        public string? PublisherStatus { get; set; }

        public List<PublisherAddressResponse>? Addresses { get; set; }

        public List<string>? Books { get; set; }
    }
}

